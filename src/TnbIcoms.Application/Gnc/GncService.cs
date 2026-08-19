using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Email;
using TnbIcoms.Application.Gnc.Dtos;
using TnbIcoms.Application.Outages;
using TnbIcoms.Application.Outages.Dtos;
using TnbIcoms.Domain.Entities.Outage;
using TnbIcoms.Infrastructure.Persistence;
using OutageEntity = TnbIcoms.Domain.Entities.Outage.Outage;
using OutagePicEntity = TnbIcoms.Domain.Entities.Outage.OutagePic;
using OutageAdditionalEquipmentEntity = TnbIcoms.Domain.Entities.Outage.OutageAdditionalEquipment;

namespace TnbIcoms.Application.Gnc;

public class GncService : IGncService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailSender _emailSender;

    public GncService(AppDbContext dbContext, IEmailSender emailSender)
    {
        _dbContext = dbContext;
        _emailSender = emailSender;
    }

    private IQueryable<OutageEntity> BaseQuery(int? zoneId)
    {
        var query = _dbContext.Outages
            .Include(o => o.Zone)
            .Include(o => o.Station)
            .Include(o => o.VoltageLevel)
            .Include(o => o.PrimaryEquipment)
            .Include(o => o.Project)
            .Include(o => o.Pics)
            .Include(o => o.CreatedByUser)
            .Include(o => o.Authorisations)
            .Where(o => !o.IsDeleted);

        if (zoneId.HasValue) query = query.Where(o => o.ZoneId == zoneId.Value);
        return query;
    }

    public async Task<ApiResponse<List<GncOutageListItemDto>>> ListScheduledAsync(int? zoneId)
    {
        var outages = await BaseQuery(zoneId)
            .Where(o => o.GnmStatus == "Approved" && o.GncStatus == null)
            .OrderBy(o => o.PlannedStartAt)
            .ToListAsync();

        var dropdownLabels = await GetDropdownLabelsAsync();
        var personnelNames = await GetPersonnelNamesAsync();
        return ApiResponse<List<GncOutageListItemDto>>.Ok(outages.Select(o => Map(o, dropdownLabels, personnelNames)).ToList());
    }

    public async Task<ApiResponse<List<GncOutageListItemDto>>> ListActiveAsync(int? zoneId)
    {
        var outages = await BaseQuery(zoneId)
            .Where(o => o.GncStatus == "Taken-Active" || o.GncStatus == "Taken-Completed")
            .OrderBy(o => o.PlannedStartAt)
            .ToListAsync();

        var dropdownLabels = await GetDropdownLabelsAsync();
        var personnelNames = await GetPersonnelNamesAsync();
        return ApiResponse<List<GncOutageListItemDto>>.Ok(outages.Select(o => Map(o, dropdownLabels, personnelNames)).ToList());
    }

    public async Task<ApiResponse<List<GncOutageListItemDto>>> ListAuthorisationInForceAsync(int? zoneId)
    {
        var outages = await BaseQuery(zoneId)
            .Where(o => o.GncStatus == "Taken-Active")
            .OrderBy(o => o.PlannedStartAt)
            .ToListAsync();

        var dropdownLabels = await GetDropdownLabelsAsync();
        var personnelNames = await GetPersonnelNamesAsync();
        return ApiResponse<List<GncOutageListItemDto>>.Ok(outages.Select(o => Map(o, dropdownLabels, personnelNames)).ToList());
    }

    public async Task<ApiResponse<object>> TakeActiveAsync(int outageId, TakeActiveRequestDto request, int currentUserId)
    {
        var outage = await _dbContext.Outages.Include(o => o.Pics).FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GnmStatus != "Approved" || outage.GncStatus is not null)
        {
            return ApiResponse<object>.Fail("Only GNM-approved outages awaiting authorisation can be taken active.");
        }

        var personnel = await _dbContext.AuthorisationPersonnel.FirstOrDefaultAsync(p => p.AuthorisationPersonnelId == request.PersonnelId && p.IsActive);
        if (personnel is null) return ApiResponse<object>.Fail("Selected authorising personnel does not exist.");

        var authorisation = new Authorisation
        {
            OutageId = outageId,
            AuthorisationNo = request.AuthorisationNo,
            PersonnelId = request.PersonnelId,
            TakenActiveAt = request.TakenActiveAt,
            Remark = request.Remark,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Authorisations.Add(authorisation);

        outage.GncStatus = "Taken-Active";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        await NotifyAuthoriserAsync(personnel.Email, outage.OutageNumber, "Taken-Active");
        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> CompleteAsync(int outageId, CompleteAuthorisationRequestDto request, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GncStatus != "Taken-Active") return ApiResponse<object>.Fail("Only Taken-Active outages can be completed.");

        var authorisation = await _dbContext.Authorisations
            .Where(a => a.OutageId == outageId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();
        if (authorisation is null) return ApiResponse<object>.Fail("No authorisation record found for this outage.");

        authorisation.TakenCompletedAt = request.TakenCompletedAt;
        if (!string.IsNullOrWhiteSpace(request.Remark)) authorisation.Remark = request.Remark;

        outage.GncStatus = "Taken-Completed";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        var personnel = await _dbContext.AuthorisationPersonnel.FirstOrDefaultAsync(p => p.AuthorisationPersonnelId == authorisation.PersonnelId);
        if (personnel is not null) await NotifyAuthoriserAsync(personnel.Email, outage.OutageNumber, "Taken-Completed");

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> ExtendAsync(int outageId, ExtendAuthorisationRequestDto request, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GncStatus != "Taken-Active") return ApiResponse<object>.Fail("Only Taken-Active outages can be extended.");

        var authorisation = await _dbContext.Authorisations
            .Where(a => a.OutageId == outageId)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();
        if (authorisation is null) return ApiResponse<object>.Fail("No authorisation record found for this outage.");

        authorisation.ExtendedTo = request.ExtendedTo;
        if (!string.IsNullOrWhiteSpace(request.Remark)) authorisation.Remark = request.Remark;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> CloseAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GncStatus != "Taken-Completed") return ApiResponse<object>.Fail("Only Taken-Completed outages can be closed.");

        outage.GncStatus = "Outage Closed";
        outage.ActualEndAt ??= DateTime.UtcNow;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> NotTakenAsync(int outageId, NotTakenRequestDto request, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GnmStatus != "Approved" || outage.GncStatus is not null)
        {
            return ApiResponse<object>.Fail("Only GNM-approved outages awaiting authorisation can be marked Not Taken.");
        }

        outage.GncStatus = "Outage Closed - Not Taken";
        outage.NotTakenReasonId = request.NotTakenReasonId;
        outage.Remark = request.Remark;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> CancelAsync(int outageId, CancelOutageRequestDto request, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GnmStatus != "Approved" || outage.GncStatus is not null)
        {
            return ApiResponse<object>.Fail("Only GNM-approved outages awaiting authorisation can be cancelled.");
        }

        outage.GncStatus = "Outage Closed - Cancelled by GNC";
        outage.Remark = request.Remark;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<OutageDetailDto>> CreateForcedOutageAsync(ForcedOutageRequestDto request, int currentUserId)
    {
        var station = await _dbContext.Stations.FirstOrDefaultAsync(s => s.StationId == request.StationId && s.IsActive);
        if (station is null) return ApiResponse<OutageDetailDto>.Fail("Selected station does not exist.");

        var primaryEquipment = await _dbContext.Equipment.FirstOrDefaultAsync(e => e.EquipmentId == request.PrimaryEquipmentId && e.IsActive);
        if (primaryEquipment is null) return ApiResponse<OutageDetailDto>.Fail("Selected equipment does not exist.");

        var jobType = await _dbContext.DropdownValues.FirstOrDefaultAsync(d => d.DropdownValueId == request.JobTypeId && d.IsActive);
        if (jobType is null) return ApiResponse<OutageDetailDto>.Fail("Selected job type does not exist.");

        if (request.PlannedEndAt <= request.PlannedStartAt)
        {
            return ApiResponse<OutageDetailDto>.Fail("End date/time must be after the start date/time.");
        }

        var outageClass = jobType.ValueLabel.Equals("Routine Maintenance", StringComparison.OrdinalIgnoreCase) ? "Maintenance" : "Project";
        var outageNumber = await GenerateOutageNumberAsync();

        var outage = new OutageEntity
        {
            OutageNumber = outageNumber,
            OutageCode = 'F',
            OutageTypeCode = "Forced",
            OutageClass = outageClass,
            WorkTypeCode = request.WorkTypeCode,
            ZoneId = station.ZoneId,
            StationId = request.StationId,
            VoltageLevelId = request.VoltageLevelId,
            PrimaryEquipmentId = request.PrimaryEquipmentId,
            LineFilterType = request.LineFilterType,
            JobTypeId = request.JobTypeId,
            PlannedStartAt = request.PlannedStartAt,
            PlannedEndAt = request.PlannedEndAt,
            ActualStartAt = request.PlannedStartAt,
            HasPtw = request.HasPtw,
            Description = request.Description,
            RequestorStatus = "Confirmed",
            PlannerStatus = "Agreed",
            GnmStatus = "Approved",
            ApprovedById = request.ApprovedById,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var equipmentId in request.AdditionalEquipmentIds.Distinct())
        {
            outage.AdditionalEquipment.Add(new OutageAdditionalEquipmentEntity { EquipmentId = equipmentId });
        }
        foreach (var pic in request.Pics)
        {
            outage.Pics.Add(new OutagePicEntity { PicName = pic.PicName, PicEmail = pic.PicEmail, PicPhone = pic.PicPhone });
        }

        _dbContext.Outages.Add(outage);
        await _dbContext.SaveChangesAsync();

        var full = await _dbContext.Outages
            .Include(o => o.Zone).Include(o => o.Station).Include(o => o.VoltageLevel)
            .Include(o => o.PrimaryEquipment).Include(o => o.Project).Include(o => o.Pics)
            .Include(o => o.AdditionalEquipment).Include(o => o.CreatedByUser)
            .FirstAsync(o => o.OutageId == outage.OutageId);

        var dropdownLabels = await GetDropdownLabelsAsync();
        var listItem = OutageService.MapListItem(full, dropdownLabels);

        return ApiResponse<OutageDetailDto>.Ok(new OutageDetailDto
        {
            OutageId = listItem.OutageId,
            OutageNumber = listItem.OutageNumber,
            OutageCode = listItem.OutageCode,
            OutageTypeCode = listItem.OutageTypeCode,
            OutageClass = listItem.OutageClass,
            WorkTypeCode = listItem.WorkTypeCode,
            ZoneName = listItem.ZoneName,
            StationName = listItem.StationName,
            VoltageLevelName = listItem.VoltageLevelName,
            EquipmentName = listItem.EquipmentName,
            JobTypeLabel = listItem.JobTypeLabel,
            PlannedStartAt = listItem.PlannedStartAt,
            PlannedEndAt = listItem.PlannedEndAt,
            RequestorStatus = listItem.RequestorStatus,
            PlannerStatus = listItem.PlannerStatus,
            GnmStatus = listItem.GnmStatus,
            GncStatus = listItem.GncStatus,
            Description = listItem.Description,
            PicNames = listItem.PicNames,
            CreatedByName = listItem.CreatedByName,
            CreatedAt = listItem.CreatedAt,
            ZoneId = full.ZoneId,
            StationId = full.StationId,
            VoltageLevelId = full.VoltageLevelId,
            PrimaryEquipmentId = full.PrimaryEquipmentId,
            EquipmentTypeId = request.EquipmentTypeId,
            AdditionalEquipmentIds = full.AdditionalEquipment.Select(a => a.EquipmentId).ToList(),
            JobTypeId = full.JobTypeId,
            HasPtw = full.HasPtw,
            Pics = full.Pics.Select(p => new OutagePicDto { PicName = p.PicName, PicEmail = p.PicEmail, PicPhone = p.PicPhone }).ToList()
        });
    }

    private async Task<string> GenerateOutageNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"ICOMS-{year}-";
        var count = await _dbContext.Outages.CountAsync(o => o.OutageNumber.StartsWith(prefix));
        return $"{prefix}{(count + 1):D5}";
    }

    private async Task<Dictionary<int, string>> GetDropdownLabelsAsync()
    {
        return await _dbContext.DropdownValues.ToDictionaryAsync(d => d.DropdownValueId, d => d.ValueLabel);
    }

    private async Task<Dictionary<int, string>> GetPersonnelNamesAsync()
    {
        return await _dbContext.AuthorisationPersonnel.ToDictionaryAsync(p => p.AuthorisationPersonnelId, p => p.FullName);
    }

    private async Task NotifyAuthoriserAsync(string email, string outageNumber, string status)
    {
        if (string.IsNullOrWhiteSpace(email)) return;
        var subject = $"Outage {outageNumber} — {status}";
        var body = $"<p>Outage {outageNumber} has been marked {status}.</p><p>Log in to ICOMS 2.0 for details.</p>";
        await _emailSender.SendAsync(email, subject, body);
    }

    private static GncOutageListItemDto Map(OutageEntity outage, Dictionary<int, string> dropdownLabels, Dictionary<int, string> personnelNames)
    {
        var listItem = OutageService.MapListItem(outage, dropdownLabels);
        var latestAuth = outage.Authorisations.OrderByDescending(a => a.CreatedAt).FirstOrDefault();

        var timelineStatus = "OnTime";
        if (outage.GncStatus == "Taken-Active")
        {
            var effectiveEnd = latestAuth?.ExtendedTo ?? outage.PlannedEndAt;
            if (latestAuth?.ExtendedTo.HasValue == true && DateTime.UtcNow <= effectiveEnd)
            {
                timelineStatus = "Extended";
            }
            else if (DateTime.UtcNow > effectiveEnd)
            {
                timelineStatus = "Overdue";
            }
        }

        return new GncOutageListItemDto
        {
            OutageId = listItem.OutageId,
            OutageNumber = listItem.OutageNumber,
            OutageCode = listItem.OutageCode,
            OutageTypeCode = listItem.OutageTypeCode,
            OutageClass = listItem.OutageClass,
            WorkTypeCode = listItem.WorkTypeCode,
            ZoneName = listItem.ZoneName,
            StationName = listItem.StationName,
            VoltageLevelName = listItem.VoltageLevelName,
            EquipmentName = listItem.EquipmentName,
            JobTypeLabel = listItem.JobTypeLabel,
            ProjectName = listItem.ProjectName,
            PlannedStartAt = listItem.PlannedStartAt,
            PlannedEndAt = listItem.PlannedEndAt,
            RestorationLabel = listItem.RestorationLabel,
            RequestorStatus = listItem.RequestorStatus,
            PlannerStatus = listItem.PlannerStatus,
            GnmStatus = listItem.GnmStatus,
            GncStatus = listItem.GncStatus,
            Description = listItem.Description,
            PicNames = listItem.PicNames,
            Justification = listItem.Justification,
            Highlights = listItem.Highlights,
            Remark = listItem.Remark,
            UnderStudyNotes = listItem.UnderStudyNotes,
            CreatedByName = listItem.CreatedByName,
            CreatedAt = listItem.CreatedAt,
            AuthorisationNo = latestAuth?.AuthorisationNo,
            TakenActiveAt = latestAuth?.TakenActiveAt,
            TakenCompletedAt = latestAuth?.TakenCompletedAt,
            ExtendedTo = latestAuth?.ExtendedTo,
            AuthoriserName = latestAuth is not null && personnelNames.TryGetValue(latestAuth.PersonnelId, out var name) ? name : null,
            TimelineStatus = timelineStatus
        };
    }
}
