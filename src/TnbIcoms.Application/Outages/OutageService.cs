using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Email;
using TnbIcoms.Application.EmailTemplates;
using TnbIcoms.Application.Outages.Dtos;
using TnbIcoms.Infrastructure.Persistence;
using OutageEntity = TnbIcoms.Domain.Entities.Outage.Outage;
using OutagePicEntity = TnbIcoms.Domain.Entities.Outage.OutagePic;
using OutageAdditionalEquipmentEntity = TnbIcoms.Domain.Entities.Outage.OutageAdditionalEquipment;

namespace TnbIcoms.Application.Outages;

public class OutageService : IOutageService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplateService;

    public OutageService(AppDbContext dbContext, IEmailSender emailSender, IEmailTemplateService emailTemplateService)
    {
        _dbContext = dbContext;
        _emailSender = emailSender;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<ApiResponse<List<OutageListItemDto>>> ListAsync(OutageListFilter filter)
    {
        var query = _dbContext.Outages
            .Include(o => o.Zone)
            .Include(o => o.Station)
            .Include(o => o.VoltageLevel)
            .Include(o => o.PrimaryEquipment)
            .Include(o => o.Project)
            .Include(o => o.Pics)
            .Include(o => o.CreatedByUser)
            .Where(o => !o.IsDeleted)
            .AsQueryable();

        if (filter.ZoneId.HasValue) query = query.Where(o => o.ZoneId == filter.ZoneId.Value);
        if (!string.IsNullOrEmpty(filter.RequestorStatus)) query = query.Where(o => o.RequestorStatus == filter.RequestorStatus);
        if (!string.IsNullOrEmpty(filter.GnmStatus)) query = query.Where(o => o.GnmStatus == filter.GnmStatus);
        if (!string.IsNullOrEmpty(filter.OutageCode)) query = query.Where(o => o.OutageCode == filter.OutageCode[0]);

        if (filter.PendingPlannerReviewOnly)
        {
            query = query.Where(o => o.RequestorStatus == "Pending" && o.PlannerStatus == null);
        }
        if (filter.PendingConfirmationOnly)
        {
            query = query.Where(o => o.PlannerStatus == "Agreed" && o.RequestorStatus == "Pending");
        }
        if (filter.AgreedAndConfirmedOnly)
        {
            query = query.Where(o => o.PlannerStatus == "Agreed" && o.RequestorStatus == "Confirmed");
        }
        if (filter.PendingGnmApprovalOnly)
        {
            query = query.Where(o => o.RequestorStatus == "Confirmed" && o.PlannerStatus == "Agreed"
                                      && (o.GnmStatus == "Pending" || o.GnmStatus == "Under-Study"));
        }
        if (filter.RangeStart.HasValue && filter.RangeEnd.HasValue)
        {
            query = query.Where(o => o.PlannedStartAt < filter.RangeEnd.Value && filter.RangeStart.Value < o.PlannedEndAt);
        }
        if (filter.StationId.HasValue) query = query.Where(o => o.StationId == filter.StationId.Value);
        if (filter.JobTypeId.HasValue) query = query.Where(o => o.JobTypeId == filter.JobTypeId.Value);
        if (filter.DateStart.HasValue) query = query.Where(o => o.PlannedStartAt >= filter.DateStart.Value);
        if (filter.DateEnd.HasValue) query = query.Where(o => o.PlannedStartAt <= filter.DateEnd.Value);
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            var keyword = filter.Keyword.Trim();
            query = query.Where(o => o.OutageNumber.Contains(keyword) || o.Description.Contains(keyword));
        }
        if (!filter.ShowDraft) query = query.Where(o => o.RequestorStatus != "Draft");

        var jobTypeLabels = await GetDropdownLabelsAsync();

        var orderedQuery = filter.SortBy == "code"
            ? query.OrderBy(o => o.OutageNumber)
            : query.OrderByDescending(o => o.PlannedStartAt);

        var outages = await orderedQuery.ToListAsync();

        return ApiResponse<List<OutageListItemDto>>.Ok(outages.Select(o => MapListItem(o, jobTypeLabels)).ToList());
    }

    public async Task<ApiResponse<OutageDetailDto>> GetByIdAsync(int outageId)
    {
        var outage = await LoadFullAsync(outageId);
        if (outage is null)
        {
            return ApiResponse<OutageDetailDto>.Fail("Outage not found.");
        }

        var jobTypeLabels = await GetDropdownLabelsAsync();
        var conflicts = await FindConflictingOutagesAsync(outage.PrimaryEquipmentId, outage.PlannedStartAt, outage.PlannedEndAt, outage.OutageId);

        return ApiResponse<OutageDetailDto>.Ok(MapDetail(outage, jobTypeLabels, conflicts, new List<string>()));
    }

    public async Task<ApiResponse<OutageDetailDto>> CreateAsync(CreateOutageRequestDto request, int requestorUserId)
    {
        var requestor = await _dbContext.AppUsers.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == requestorUserId);
        if (requestor is null)
        {
            return ApiResponse<OutageDetailDto>.Fail("Requesting user not found.");
        }

        var station = await _dbContext.Stations.FirstOrDefaultAsync(s => s.StationId == request.StationId && s.IsActive);
        if (station is null)
        {
            return ApiResponse<OutageDetailDto>.Fail("Selected station does not exist.");
        }

        var voltageLevel = await _dbContext.VoltageLevels.FirstOrDefaultAsync(v => v.VoltageLevelId == request.VoltageLevelId && v.IsActive);
        if (voltageLevel is null)
        {
            return ApiResponse<OutageDetailDto>.Fail("Selected voltage level does not exist.");
        }

        var primaryEquipment = await _dbContext.Equipment
            .FirstOrDefaultAsync(e => e.EquipmentId == request.PrimaryEquipmentId && e.IsActive);
        if (primaryEquipment is null)
        {
            return ApiResponse<OutageDetailDto>.Fail("Selected equipment does not exist.");
        }

        var jobType = await _dbContext.DropdownValues.FirstOrDefaultAsync(d => d.DropdownValueId == request.JobTypeId && d.IsActive);
        if (jobType is null)
        {
            return ApiResponse<OutageDetailDto>.Fail("Selected job type does not exist.");
        }

        var outageClass = jobType.ValueLabel.Equals("Routine Maintenance", StringComparison.OrdinalIgnoreCase) ? "Maintenance" : "Project";
        if (outageClass == "Project" && !request.ProjectId.HasValue)
        {
            return ApiResponse<OutageDetailDto>.Fail("A project must be selected for this job type.");
        }

        if (request.PlannedEndAt <= request.PlannedStartAt)
        {
            return ApiResponse<OutageDetailDto>.Fail("End date/time must be after the start date/time.");
        }

        if (request.SubmitNow && string.IsNullOrWhiteSpace(request.Description))
        {
            return ApiResponse<OutageDetailDto>.Fail("Description is required.");
        }

        var (outageTypeCode, classifyError) = await ClassifyOutageTypeAsync(request.WorkTypeCode, voltageLevel.LevelName, request.PlannedStartAt);
        if (classifyError is not null)
        {
            return ApiResponse<OutageDetailDto>.Fail(classifyError);
        }

        // URS Module 2 §5.6: exact equipment + job type + overlapping date is redundant and blocked outright.
        var redundant = await _dbContext.Outages.AnyAsync(o =>
            !o.IsDeleted && o.PrimaryEquipmentId == request.PrimaryEquipmentId && o.JobTypeId == request.JobTypeId &&
            !o.RequestorStatus.StartsWith("Outage Closed") && !o.GnmStatus.StartsWith("Outage Closed") &&
            o.PlannedStartAt < request.PlannedEndAt && request.PlannedStartAt < o.PlannedEndAt);
        if (redundant)
        {
            return ApiResponse<OutageDetailDto>.Fail("An outage already exists for this equipment, job type, and overlapping dates.");
        }

        // URS Module 2 §5.6: a zone with an unresolved "Outage Closed - Not Taken" outage is locked from creating new ones.
        var notTakenLock = await _dbContext.Outages.AnyAsync(o =>
            !o.IsDeleted && o.ZoneId == station.ZoneId && o.GncStatus == "Outage Closed - Not Taken" && o.NotTakenReasonId == null);
        if (notTakenLock)
        {
            return ApiResponse<OutageDetailDto>.Fail("This zone has an outage marked Not Taken awaiting a reason. Resolve it before creating new outages.");
        }

        var warnings = new List<string>();
        var kivMatch = await _dbContext.Outages.AnyAsync(o => !o.IsDeleted && o.PrimaryEquipmentId == request.PrimaryEquipmentId && o.GnmStatus == "KIV");
        if (kivMatch)
        {
            warnings.Add("A KIV outage already exists for this equipment. Consider submitting a Change Request against it instead of creating a new one.");
        }

        var outageNumber = await GenerateOutageNumberAsync();
        var roleCode = requestor.Role?.RoleCode ?? string.Empty;
        var isEmergency = outageTypeCode == "Emergency";
        var isPlannerOrGnm = roleCode is "PLANNER" or "GNM" or "GNM_ADMIN";

        var requestorStatus = request.SubmitNow ? "Pending" : "Draft";
        string? plannerStatus = null;

        if (request.SubmitNow)
        {
            if (isEmergency)
            {
                plannerStatus = "Agreed";
                requestorStatus = "Confirmed";
            }
            else if (isPlannerOrGnm)
            {
                plannerStatus = "Agreed";
            }
        }

        var outage = new OutageEntity
        {
            OutageNumber = outageNumber,
            OutageCode = outageTypeCode[0],
            OutageTypeCode = outageTypeCode,
            OutageClass = outageClass,
            WorkTypeCode = request.WorkTypeCode,
            ZoneId = station.ZoneId,
            StationId = request.StationId,
            VoltageLevelId = request.VoltageLevelId,
            PrimaryEquipmentId = request.PrimaryEquipmentId,
            LineFilterType = request.LineFilterType,
            JobTypeId = request.JobTypeId,
            ProjectId = request.ProjectId,
            SequenceId = request.SequenceId,
            RestorationId = request.RestorationId,
            PlannedStartAt = request.PlannedStartAt,
            PlannedEndAt = request.PlannedEndAt,
            HasPtw = request.HasPtw,
            Description = request.Description,
            RequestorStatus = requestorStatus,
            PlannerStatus = plannerStatus,
            GnmStatus = "Pending",
            CreatedBy = requestorUserId,
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

        if (request.SubmitNow)
        {
            await NotifyPicsAsync(outage, "created");
        }

        var full = await LoadFullAsync(outage.OutageId);
        var jobTypeLabels = await GetDropdownLabelsAsync();
        var conflicts = await FindConflictingOutagesAsync(outage.PrimaryEquipmentId, outage.PlannedStartAt, outage.PlannedEndAt, outage.OutageId);

        return ApiResponse<OutageDetailDto>.Ok(MapDetail(full!, jobTypeLabels, conflicts, warnings));
    }

    public async Task<ApiResponse<object>> SubmitDraftAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.RequestorStatus != "Draft") return ApiResponse<object>.Fail("Only draft outages can be submitted.");
        if (string.IsNullOrWhiteSpace(outage.Description)) return ApiResponse<object>.Fail("Description is required before submitting.");

        var requestor = await _dbContext.AppUsers.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == currentUserId);
        var roleCode = requestor?.Role?.RoleCode ?? string.Empty;
        var isEmergency = outage.OutageTypeCode == "Emergency";
        var isPlannerOrGnm = roleCode is "PLANNER" or "GNM" or "GNM_ADMIN";

        outage.RequestorStatus = "Pending";
        if (isEmergency)
        {
            outage.PlannerStatus = "Agreed";
            outage.RequestorStatus = "Confirmed";
        }
        else if (isPlannerOrGnm)
        {
            outage.PlannerStatus = "Agreed";
        }

        await _dbContext.SaveChangesAsync();
        await NotifyPicsAsync(outage, "submitted");

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> AgreeAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.RequestorStatus != "Pending" || outage.PlannerStatus is not null)
        {
            return ApiResponse<object>.Fail("This outage is not awaiting Planner review.");
        }

        outage.PlannerStatus = "Agreed";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> DisagreeAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.RequestorStatus != "Pending" || outage.PlannerStatus is not null)
        {
            return ApiResponse<object>.Fail("This outage is not awaiting Planner review.");
        }

        outage.RequestorStatus = "Draft";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> ConfirmAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.PlannerStatus != "Agreed" || outage.RequestorStatus != "Pending")
        {
            return ApiResponse<object>.Fail("This outage is not awaiting Requestor confirmation.");
        }

        outage.RequestorStatus = "Confirmed";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> RejectAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.PlannerStatus != "Agreed" || outage.RequestorStatus != "Pending")
        {
            return ApiResponse<object>.Fail("This outage is not awaiting Requestor confirmation.");
        }

        outage.IsDeleted = true;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> StartStudyAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GnmStatus != "Pending") return ApiResponse<object>.Fail("Only Pending outages can start study.");

        outage.GnmStatus = "Under-Study";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> UpdateStudyAsync(int outageId, StudyUpdateRequestDto request, bool notify, int currentUserId)
    {
        var outage = await _dbContext.Outages.Include(o => o.Pics).FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");

        outage.Justification = request.Justification;
        outage.Highlights = request.Highlights;
        outage.Remark = request.Remark;
        outage.UnderStudyNotes = request.UnderStudyNotes;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        if (notify)
        {
            await NotifyPicsAsync(outage, "updated by GNM");
        }

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> SetKivAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GnmStatus is not ("Pending" or "Under-Study")) return ApiResponse<object>.Fail("Only Pending or Under-Study outages can be set to KIV.");

        outage.GnmStatus = "KIV";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> ApproveAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.RequestorStatus != "Confirmed" || outage.PlannerStatus != "Agreed")
        {
            return ApiResponse<object>.Fail("This outage is not awaiting GNM approval.");
        }
        if (outage.GnmStatus is not ("Pending" or "Under-Study" or "KIV"))
        {
            return ApiResponse<object>.Fail($"Cannot approve an outage with GNM status '{outage.GnmStatus}'.");
        }

        outage.GnmStatus = "Approved";
        outage.ApprovedById = currentUserId;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> DisapproveAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GnmStatus is not ("Pending" or "Under-Study" or "KIV"))
        {
            return ApiResponse<object>.Fail($"Cannot disapprove an outage with GNM status '{outage.GnmStatus}'.");
        }

        outage.GnmStatus = "Disapproved";
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    /// <summary>
    /// URS §5.4: an Approved outage can be backtracked to Under-Study for re-review,
    /// but only before GNC has taken it active.
    /// </summary>
    public async Task<ApiResponse<object>> RevertApprovalAsync(int outageId, int currentUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
        if (outage is null) return ApiResponse<object>.Fail("Outage not found.");
        if (outage.GnmStatus != "Approved") return ApiResponse<object>.Fail("Only Approved outages can be reverted.");
        if (outage.GncStatus is not null and not "Outage Closed - Not Taken")
        {
            return ApiResponse<object>.Fail("This outage already has GNC activity and cannot be reverted.");
        }

        outage.GnmStatus = "Under-Study";
        outage.ApprovedById = null;
        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public Task<ApiResponse<BulkActionResultDto>> BulkApproveAsync(BulkActionRequestDto request, int currentUserId) =>
        RunBulkAsync(request.OutageIds, id => ApproveAsync(id, currentUserId));

    public Task<ApiResponse<BulkActionResultDto>> BulkDisapproveAsync(BulkActionRequestDto request, int currentUserId) =>
        RunBulkAsync(request.OutageIds, id => DisapproveAsync(id, currentUserId));

    public Task<ApiResponse<BulkActionResultDto>> BulkAgreeAsync(BulkActionRequestDto request, int currentUserId) =>
        RunBulkAsync(request.OutageIds, id => AgreeAsync(id, currentUserId));

    public Task<ApiResponse<BulkActionResultDto>> BulkDisagreeAsync(BulkActionRequestDto request, int currentUserId) =>
        RunBulkAsync(request.OutageIds, id => DisagreeAsync(id, currentUserId));

    public Task<ApiResponse<BulkActionResultDto>> BulkConfirmAsync(BulkActionRequestDto request, int currentUserId) =>
        RunBulkAsync(request.OutageIds, id => ConfirmAsync(id, currentUserId));

    public Task<ApiResponse<BulkActionResultDto>> BulkRejectAsync(BulkActionRequestDto request, int currentUserId) =>
        RunBulkAsync(request.OutageIds, id => RejectAsync(id, currentUserId));

    private async Task<ApiResponse<BulkActionResultDto>> RunBulkAsync(List<int> ids, Func<int, Task<ApiResponse<object>>> action)
    {
        var result = new BulkActionResultDto();
        foreach (var id in ids)
        {
            var single = await action(id);
            if (single.Success) result.SucceededCount++;
            else result.Errors.Add($"#{id}: {single.Error}");
        }
        return ApiResponse<BulkActionResultDto>.Ok(result);
    }

    /// <summary>
    /// URS Module 1 §5.2.8: outage type is classified from lead time (days between now and the
    /// planned start) against admin-configured rules for the selected work type and voltage.
    /// </summary>
    private async Task<(string Code, string? Error)> ClassifyOutageTypeAsync(string workTypeCode, string voltageLevelName, DateTime plannedStartAt)
    {
        var leadDays = (plannedStartAt.Date - DateTime.UtcNow.Date).Days;

        var rules = await _dbContext.OutageTypeRules
            .Where(r => r.IsActive && r.WorkTypeCode == workTypeCode)
            .ToListAsync();

        foreach (var rule in rules)
        {
            var appliesToAll = rule.AppliesTo.Trim().Equals("ALL", StringComparison.OrdinalIgnoreCase);
            var appliesToThis = appliesToAll || rule.AppliesTo.Split(',', StringSplitOptions.TrimEntries)
                .Any(v => v.Equals(voltageLevelName, StringComparison.OrdinalIgnoreCase));
            if (!appliesToThis) continue;

            var moreThan = (rule.MoreThanDays ?? 0) + (rule.MoreThanMonths ?? 0) * 30 + (rule.MoreThanYears ?? 0) * 365;
            var lessThanRaw = (rule.LessThanDays ?? 0) + (rule.LessThanMonths ?? 0) * 30 + (rule.LessThanYears ?? 0) * 365;
            var lessThan = lessThanRaw > 0 ? lessThanRaw : int.MaxValue;

            if (leadDays > moreThan && leadDays < lessThan)
            {
                return (rule.OutageTypeCode, null);
            }
        }

        return (string.Empty, "No configured Outage Type rule matches this work type, voltage, and date. Ask GNM ADMIN to configure one under Outage Type Configuration.");
    }

    private async Task<List<ConflictingOutageDto>> FindConflictingOutagesAsync(int primaryEquipmentId, DateTime start, DateTime end, int? excludeOutageId)
    {
        var pairedEquipmentIds = await _dbContext.ConflictingLines
            .Where(c => c.IsActive && (c.EquipmentId == primaryEquipmentId || c.ConflictingEquipmentId == primaryEquipmentId))
            .Select(c => c.EquipmentId == primaryEquipmentId ? c.ConflictingEquipmentId : c.EquipmentId)
            .ToListAsync();

        if (pairedEquipmentIds.Count == 0)
        {
            return new List<ConflictingOutageDto>();
        }

        var conflicts = await _dbContext.Outages
            .Include(o => o.PrimaryEquipment)
            .Where(o => !o.IsDeleted && (excludeOutageId == null || o.OutageId != excludeOutageId)
                        && pairedEquipmentIds.Contains(o.PrimaryEquipmentId)
                        && !o.RequestorStatus.StartsWith("Outage Closed") && !o.GnmStatus.StartsWith("Outage Closed")
                        && o.PlannedStartAt < end && start < o.PlannedEndAt)
            .Select(o => new ConflictingOutageDto
            {
                OutageId = o.OutageId,
                OutageNumber = o.OutageNumber,
                EquipmentName = o.PrimaryEquipment!.EquipmentName,
                PlannedStartAt = o.PlannedStartAt,
                PlannedEndAt = o.PlannedEndAt
            })
            .ToListAsync();

        return conflicts;
    }

    private async Task<string> GenerateOutageNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"ICOMS-{year}-";
        var count = await _dbContext.Outages.CountAsync(o => o.OutageNumber.StartsWith(prefix));
        return $"{prefix}{(count + 1):D5}";
    }

    private async Task NotifyPicsAsync(OutageEntity outage, string action)
    {
        const string templateCode = "OutageStatusNotification";
        var rendered = await _emailTemplateService.RenderAsync(templateCode, new Dictionary<string, string>
        {
            ["OutageNumber"] = outage.OutageNumber,
            ["Action"] = action
        });
        if (rendered is null) return; // template inactive/missing — admin has suppressed this notification

        var pics = outage.Pics.Count > 0
            ? outage.Pics
            : await _dbContext.OutagePics.Where(p => p.OutageId == outage.OutageId).ToListAsync();

        foreach (var pic in pics)
        {
            if (!string.IsNullOrWhiteSpace(pic.PicEmail))
            {
                await _emailSender.SendAsync(pic.PicEmail, rendered.Value.Subject, rendered.Value.Body, templateCode);
            }
        }
    }

    private Task<OutageEntity?> LoadFullAsync(int outageId)
    {
        return _dbContext.Outages
            .Include(o => o.Zone)
            .Include(o => o.Station)
            .Include(o => o.VoltageLevel)
            .Include(o => o.PrimaryEquipment)
            .Include(o => o.Project)
            .Include(o => o.Pics)
            .Include(o => o.AdditionalEquipment)
            .Include(o => o.CreatedByUser)
            .FirstOrDefaultAsync(o => o.OutageId == outageId && !o.IsDeleted);
    }

    private async Task<Dictionary<int, string>> GetDropdownLabelsAsync()
    {
        return await _dbContext.DropdownValues.ToDictionaryAsync(d => d.DropdownValueId, d => d.ValueLabel);
    }

    internal static OutageListItemDto MapListItem(OutageEntity outage, Dictionary<int, string> dropdownLabels)
    {
        return new OutageListItemDto
        {
            OutageId = outage.OutageId,
            OutageNumber = outage.OutageNumber,
            OutageCode = outage.OutageCode,
            OutageTypeCode = outage.OutageTypeCode,
            OutageClass = outage.OutageClass,
            WorkTypeCode = outage.WorkTypeCode,
            ZoneName = outage.Zone?.ZoneName,
            StationName = outage.Station?.StationName,
            VoltageLevelName = outage.VoltageLevel?.LevelName,
            EquipmentName = outage.PrimaryEquipment?.EquipmentName,
            JobTypeLabel = dropdownLabels.TryGetValue(outage.JobTypeId, out var jt) ? jt : null,
            ProjectName = outage.Project?.ProjectName,
            PlannedStartAt = outage.PlannedStartAt,
            PlannedEndAt = outage.PlannedEndAt,
            RestorationLabel = outage.RestorationId.HasValue && dropdownLabels.TryGetValue(outage.RestorationId.Value, out var r) ? r : null,
            RequestorStatus = outage.RequestorStatus,
            PlannerStatus = outage.PlannerStatus,
            GnmStatus = outage.GnmStatus,
            GncStatus = outage.GncStatus,
            Description = outage.Description,
            PicNames = outage.Pics.Select(p => p.PicName).ToList(),
            Justification = outage.Justification,
            Highlights = outage.Highlights,
            Remark = outage.Remark,
            UnderStudyNotes = outage.UnderStudyNotes,
            CreatedByName = outage.CreatedByUser?.FullName ?? string.Empty,
            CreatedAt = outage.CreatedAt
        };
    }

    private static OutageDetailDto MapDetail(OutageEntity outage, Dictionary<int, string> dropdownLabels, List<ConflictingOutageDto> conflicts, List<string> warnings)
    {
        var listItem = MapListItem(outage, dropdownLabels);

        return new OutageDetailDto
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
            ZoneId = outage.ZoneId,
            StationId = outage.StationId,
            VoltageLevelId = outage.VoltageLevelId,
            PrimaryEquipmentId = outage.PrimaryEquipmentId,
            EquipmentTypeId = outage.PrimaryEquipment?.EquipmentTypeId ?? 0,
            AdditionalEquipmentIds = outage.AdditionalEquipment.Select(a => a.EquipmentId).ToList(),
            SequenceId = outage.SequenceId,
            RestorationId = outage.RestorationId,
            JobTypeId = outage.JobTypeId,
            ProjectId = outage.ProjectId,
            HasPtw = outage.HasPtw,
            Pics = outage.Pics.Select(p => new OutagePicDto { PicName = p.PicName, PicEmail = p.PicEmail, PicPhone = p.PicPhone }).ToList(),
            ConflictingOutages = conflicts,
            Warnings = warnings
        };
    }
}
