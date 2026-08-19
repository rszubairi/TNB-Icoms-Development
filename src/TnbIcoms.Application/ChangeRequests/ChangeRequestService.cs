using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.ChangeRequests.Dtos;
using TnbIcoms.Application.Common;
using TnbIcoms.Infrastructure.Persistence;
using ChangeRequestEntity = TnbIcoms.Domain.Entities.Outage.ChangeRequest;
using OutageAdditionalEquipmentEntity = TnbIcoms.Domain.Entities.Outage.OutageAdditionalEquipment;

namespace TnbIcoms.Application.ChangeRequests;

public class ChangeRequestService : IChangeRequestService
{
    private readonly AppDbContext _dbContext;

    public ChangeRequestService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<ChangeRequestBatchDto>>> ListForOutageAsync(int outageId)
    {
        var rows = await _dbContext.ChangeRequests
            .Include(c => c.Outage)
            .Where(c => c.OutageId == outageId)
            .OrderByDescending(c => c.RequestedAt)
            .ToListAsync();

        return ApiResponse<List<ChangeRequestBatchDto>>.Ok(await MapBatchesAsync(rows));
    }

    public async Task<ApiResponse<List<ChangeRequestBatchDto>>> ListPendingAsync()
    {
        var rows = await _dbContext.ChangeRequests
            .Include(c => c.Outage)
            .Where(c => c.Status == "Pending")
            .OrderBy(c => c.RequestedAt)
            .ToListAsync();

        return ApiResponse<List<ChangeRequestBatchDto>>.Ok(await MapBatchesAsync(rows));
    }

    public async Task<ApiResponse<ChangeRequestBatchDto>> CreateAsync(CreateChangeRequestBatchDto request, int requestedByUserId)
    {
        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == request.OutageId && !o.IsDeleted);
        if (outage is null)
        {
            return ApiResponse<ChangeRequestBatchDto>.Fail("Outage not found.");
        }

        if (outage.GnmStatus == "Approved" || outage.GnmStatus.StartsWith("Outage Closed") || outage.RequestorStatus.StartsWith("Outage Closed"))
        {
            return ApiResponse<ChangeRequestBatchDto>.Fail("This outage has already been Approved or Closed. Changes now require a manual GNM request.");
        }

        var hasPendingBatch = await _dbContext.ChangeRequests.AnyAsync(c => c.OutageId == request.OutageId && c.Status == "Pending");
        if (hasPendingBatch)
        {
            return ApiResponse<ChangeRequestBatchDto>.Fail("A change request is already pending review for this outage.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return ApiResponse<ChangeRequestBatchDto>.Fail("A reason is required.");
        }

        var batchId = Guid.NewGuid();
        var rows = new List<ChangeRequestEntity>();

        if (request.NewPlannedStartAt.HasValue && request.NewPlannedEndAt.HasValue)
        {
            rows.Add(new ChangeRequestEntity
            {
                BatchId = batchId,
                OutageId = outage.OutageId,
                FieldName = "Schedule",
                OldValue = $"{outage.PlannedStartAt:O}|{outage.PlannedEndAt:O}",
                NewValue = $"{request.NewPlannedStartAt:O}|{request.NewPlannedEndAt:O}",
                Reason = request.Reason,
                RequestedBy = requestedByUserId
            });
        }

        if (request.NewVoltageLevelId.HasValue && request.NewVoltageLevelId != outage.VoltageLevelId)
        {
            var voltageExists = await _dbContext.VoltageLevels.AnyAsync(v => v.VoltageLevelId == request.NewVoltageLevelId && v.IsActive);
            if (!voltageExists)
            {
                return ApiResponse<ChangeRequestBatchDto>.Fail("Selected voltage level does not exist.");
            }
            rows.Add(new ChangeRequestEntity
            {
                BatchId = batchId,
                OutageId = outage.OutageId,
                FieldName = "VoltageLevel",
                OldValue = outage.VoltageLevelId.ToString(),
                NewValue = request.NewVoltageLevelId.Value.ToString(),
                Reason = request.Reason,
                RequestedBy = requestedByUserId
            });
        }

        if (request.NewPrimaryEquipmentId.HasValue && request.NewPrimaryEquipmentId != outage.PrimaryEquipmentId)
        {
            var equipmentExists = await _dbContext.Equipment.AnyAsync(e => e.EquipmentId == request.NewPrimaryEquipmentId && e.IsActive);
            if (!equipmentExists)
            {
                return ApiResponse<ChangeRequestBatchDto>.Fail("Selected equipment does not exist.");
            }
            rows.Add(new ChangeRequestEntity
            {
                BatchId = batchId,
                OutageId = outage.OutageId,
                FieldName = "PrimaryEquipment",
                OldValue = outage.PrimaryEquipmentId.ToString(),
                NewValue = request.NewPrimaryEquipmentId.Value.ToString(),
                Reason = request.Reason,
                RequestedBy = requestedByUserId
            });
        }

        if (request.AddAdditionalEquipmentIds.Count > 0)
        {
            rows.Add(new ChangeRequestEntity
            {
                BatchId = batchId,
                OutageId = outage.OutageId,
                FieldName = "AdditionalEquipment",
                OldValue = null,
                NewValue = string.Join(",", request.AddAdditionalEquipmentIds),
                Reason = request.Reason,
                RequestedBy = requestedByUserId
            });
        }

        if (rows.Count == 0)
        {
            return ApiResponse<ChangeRequestBatchDto>.Fail("Select at least one field to change.");
        }

        _dbContext.ChangeRequests.AddRange(rows);
        await _dbContext.SaveChangesAsync();

        var saved = await _dbContext.ChangeRequests.Include(c => c.Outage).Where(c => c.BatchId == batchId).ToListAsync();
        var batches = await MapBatchesAsync(saved);

        return ApiResponse<ChangeRequestBatchDto>.Ok(batches.First());
    }

    public async Task<ApiResponse<object>> ApproveAsync(Guid batchId, int reviewerUserId)
    {
        var rows = await _dbContext.ChangeRequests.Where(c => c.BatchId == batchId).ToListAsync();
        if (rows.Count == 0)
        {
            return ApiResponse<object>.Fail("Change request not found.");
        }
        if (rows.Any(r => r.Status != "Pending"))
        {
            return ApiResponse<object>.Fail("This change request has already been actioned.");
        }

        var outage = await _dbContext.Outages.FirstOrDefaultAsync(o => o.OutageId == rows[0].OutageId);
        if (outage is null)
        {
            return ApiResponse<object>.Fail("Outage no longer exists.");
        }

        foreach (var row in rows)
        {
            switch (row.FieldName)
            {
                case "Schedule":
                    var parts = row.NewValue!.Split('|');
                    outage.PlannedStartAt = DateTime.Parse(parts[0]).ToUniversalTime();
                    outage.PlannedEndAt = DateTime.Parse(parts[1]).ToUniversalTime();
                    break;
                case "VoltageLevel":
                    outage.VoltageLevelId = int.Parse(row.NewValue!);
                    break;
                case "PrimaryEquipment":
                    outage.PrimaryEquipmentId = int.Parse(row.NewValue!);
                    break;
                case "AdditionalEquipment":
                    var idsToAdd = row.NewValue!.Split(',').Select(int.Parse);
                    foreach (var id in idsToAdd)
                    {
                        _dbContext.OutageAdditionalEquipment.Add(new OutageAdditionalEquipmentEntity
                        {
                            OutageId = outage.OutageId,
                            EquipmentId = id
                        });
                    }
                    break;
            }

            row.Status = "Approved";
            row.ApprovedBy = reviewerUserId;
            row.ApprovedAt = DateTime.UtcNow;
        }

        outage.UpdatedAt = DateTime.UtcNow;
        outage.UpdatedBy = reviewerUserId;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> RejectAsync(Guid batchId, int reviewerUserId, RejectChangeRequestBatchDto request)
    {
        var rows = await _dbContext.ChangeRequests.Where(c => c.BatchId == batchId).ToListAsync();
        if (rows.Count == 0)
        {
            return ApiResponse<object>.Fail("Change request not found.");
        }
        if (rows.Any(r => r.Status != "Pending"))
        {
            return ApiResponse<object>.Fail("This change request has already been actioned.");
        }

        foreach (var row in rows)
        {
            row.Status = "Rejected";
            row.ApprovedBy = reviewerUserId;
            row.ApprovedAt = DateTime.UtcNow;
            row.ReviewComment = request.Comment;
        }

        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private async Task<List<ChangeRequestBatchDto>> MapBatchesAsync(List<ChangeRequestEntity> rows)
    {
        var userIds = rows.SelectMany(r => new[] { (int?)r.RequestedBy, r.ApprovedBy })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var userNames = await _dbContext.AppUsers
            .Where(u => userIds.Contains(u.UserId))
            .ToDictionaryAsync(u => u.UserId, u => u.FullName);

        return rows
            .GroupBy(r => r.BatchId)
            .Select(g =>
            {
                var first = g.First();
                return new ChangeRequestBatchDto
                {
                    BatchId = g.Key,
                    OutageId = first.OutageId,
                    OutageNumber = first.Outage?.OutageNumber,
                    Status = first.Status,
                    Reason = first.Reason,
                    RequestedByName = userNames.TryGetValue(first.RequestedBy, out var reqName) ? reqName : null,
                    RequestedAt = first.RequestedAt,
                    ReviewedByName = first.ApprovedBy.HasValue && userNames.TryGetValue(first.ApprovedBy.Value, out var revName) ? revName : null,
                    ReviewedAt = first.ApprovedAt,
                    ReviewComment = first.ReviewComment,
                    Fields = g.Select(f => new ChangeRequestFieldDto { FieldName = f.FieldName, OldValue = f.OldValue, NewValue = f.NewValue }).ToList()
                };
            })
            .OrderByDescending(b => b.RequestedAt)
            .ToList();
    }
}
