using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.ConflictingLines.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.ConflictingLines;

public class ConflictingLineService : IConflictingLineService
{
    private readonly AppDbContext _dbContext;

    public ConflictingLineService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<ConflictingLineDto>>> ListAsync()
    {
        var pairs = await _dbContext.ConflictingLines
            .Include(c => c.Equipment)
            .Include(c => c.ConflictingEquipment)
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.ConflictingLineId)
            .Select(c => Map(c))
            .ToListAsync();

        return ApiResponse<List<ConflictingLineDto>>.Ok(pairs);
    }

    public async Task<ApiResponse<ConflictingLineDto>> CreateAsync(CreateConflictingLineRequestDto request)
    {
        if (request.EquipmentId == request.ConflictingEquipmentId)
        {
            return ApiResponse<ConflictingLineDto>.Fail("An equipment cannot conflict with itself.");
        }

        var equipmentExist = await _dbContext.Equipment
            .CountAsync(e => (e.EquipmentId == request.EquipmentId || e.EquipmentId == request.ConflictingEquipmentId) && e.IsActive);
        if (equipmentExist != 2)
        {
            return ApiResponse<ConflictingLineDto>.Fail("Selected equipment does not exist.");
        }

        var duplicate = await _dbContext.ConflictingLines.AnyAsync(c => c.IsActive &&
            ((c.EquipmentId == request.EquipmentId && c.ConflictingEquipmentId == request.ConflictingEquipmentId) ||
             (c.EquipmentId == request.ConflictingEquipmentId && c.ConflictingEquipmentId == request.EquipmentId)));
        if (duplicate)
        {
            return ApiResponse<ConflictingLineDto>.Fail("This pair is already configured as conflicting.");
        }

        var pair = new ConflictingLine
        {
            EquipmentId = request.EquipmentId,
            ConflictingEquipmentId = request.ConflictingEquipmentId,
            Remark = request.Remark,
            IsActive = true
        };

        _dbContext.ConflictingLines.Add(pair);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(pair).Reference(p => p.Equipment).LoadAsync();
        await _dbContext.Entry(pair).Reference(p => p.ConflictingEquipment).LoadAsync();

        return ApiResponse<ConflictingLineDto>.Ok(Map(pair));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int conflictingLineId)
    {
        var pair = await _dbContext.ConflictingLines.FirstOrDefaultAsync(c => c.ConflictingLineId == conflictingLineId);
        if (pair is null)
        {
            return ApiResponse<object>.Fail("Not found.");
        }

        pair.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static ConflictingLineDto Map(ConflictingLine pair)
    {
        return new ConflictingLineDto
        {
            ConflictingLineId = pair.ConflictingLineId,
            EquipmentId = pair.EquipmentId,
            EquipmentName = pair.Equipment?.EquipmentName,
            ConflictingEquipmentId = pair.ConflictingEquipmentId,
            ConflictingEquipmentName = pair.ConflictingEquipment?.EquipmentName,
            Remark = pair.Remark,
            IsActive = pair.IsActive
        };
    }
}
