using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.VoltageLevels.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.VoltageLevels;

public class VoltageLevelService : IVoltageLevelService
{
    private readonly AppDbContext _dbContext;

    public VoltageLevelService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<VoltageLevelDto>>> ListAsync()
    {
        var levels = await _dbContext.VoltageLevels
            .OrderBy(v => v.SortOrder)
            .ThenBy(v => v.LevelName)
            .Select(v => new VoltageLevelDto
            {
                VoltageLevelId = v.VoltageLevelId,
                LevelName = v.LevelName,
                SortOrder = v.SortOrder,
                IsActive = v.IsActive,
                EquipmentTypeCount = _dbContext.EquipmentTypes.Count(t => t.VoltageLevelId == v.VoltageLevelId && t.IsActive)
            })
            .ToListAsync();

        return ApiResponse<List<VoltageLevelDto>>.Ok(levels);
    }

    public async Task<ApiResponse<VoltageLevelDto>> CreateAsync(CreateVoltageLevelRequestDto request)
    {
        var name = request.LevelName.Trim();
        if (await _dbContext.VoltageLevels.AnyAsync(v => v.LevelName == name))
        {
            return ApiResponse<VoltageLevelDto>.Fail("This voltage level already exists.");
        }

        var maxSortOrder = await _dbContext.VoltageLevels.Select(v => (int?)v.SortOrder).MaxAsync() ?? 0;

        var level = new VoltageLevel
        {
            LevelName = name,
            SortOrder = maxSortOrder + 1,
            IsActive = true
        };

        _dbContext.VoltageLevels.Add(level);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<VoltageLevelDto>.Ok(new VoltageLevelDto
        {
            VoltageLevelId = level.VoltageLevelId,
            LevelName = level.LevelName,
            SortOrder = level.SortOrder,
            IsActive = level.IsActive,
            EquipmentTypeCount = 0
        });
    }

    public async Task<ApiResponse<VoltageLevelDto>> UpdateAsync(int voltageLevelId, UpdateVoltageLevelRequestDto request)
    {
        var level = await _dbContext.VoltageLevels.FirstOrDefaultAsync(v => v.VoltageLevelId == voltageLevelId);
        if (level is null)
        {
            return ApiResponse<VoltageLevelDto>.Fail("Voltage level not found.");
        }

        var name = request.LevelName.Trim();
        if (await _dbContext.VoltageLevels.AnyAsync(v => v.VoltageLevelId != voltageLevelId && v.LevelName == name))
        {
            return ApiResponse<VoltageLevelDto>.Fail("This voltage level already exists.");
        }

        level.LevelName = name;
        level.SortOrder = request.SortOrder;
        level.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        var equipmentTypeCount = await _dbContext.EquipmentTypes.CountAsync(t => t.VoltageLevelId == voltageLevelId && t.IsActive);

        return ApiResponse<VoltageLevelDto>.Ok(new VoltageLevelDto
        {
            VoltageLevelId = level.VoltageLevelId,
            LevelName = level.LevelName,
            SortOrder = level.SortOrder,
            IsActive = level.IsActive,
            EquipmentTypeCount = equipmentTypeCount
        });
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int voltageLevelId)
    {
        var level = await _dbContext.VoltageLevels.FirstOrDefaultAsync(v => v.VoltageLevelId == voltageLevelId);
        if (level is null)
        {
            return ApiResponse<object>.Fail("Voltage level not found.");
        }

        var hasEquipmentTypes = await _dbContext.EquipmentTypes.AnyAsync(t => t.VoltageLevelId == voltageLevelId && t.IsActive);
        if (hasEquipmentTypes)
        {
            return ApiResponse<object>.Fail("This voltage level has active equipment types and cannot be deactivated.");
        }

        level.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }
}
