using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.EquipmentTypes.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.EquipmentTypes;

public class EquipmentTypeService : IEquipmentTypeService
{
    private readonly AppDbContext _dbContext;

    public EquipmentTypeService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<EquipmentTypeDto>>> ListAsync(int? voltageLevelId)
    {
        var query = _dbContext.EquipmentTypes
            .Include(t => t.VoltageLevel)
            .AsQueryable();

        if (voltageLevelId.HasValue)
        {
            query = query.Where(t => t.VoltageLevelId == voltageLevelId.Value);
        }

        var types = await query
            .OrderBy(t => t.TypeName)
            .Select(t => Map(t))
            .ToListAsync();

        return ApiResponse<List<EquipmentTypeDto>>.Ok(types);
    }

    public async Task<ApiResponse<EquipmentTypeDto>> CreateAsync(CreateEquipmentTypeRequestDto request)
    {
        var voltageLevel = await _dbContext.VoltageLevels.FirstOrDefaultAsync(v => v.VoltageLevelId == request.VoltageLevelId && v.IsActive);
        if (voltageLevel is null)
        {
            return ApiResponse<EquipmentTypeDto>.Fail("Selected voltage level does not exist.");
        }

        var name = request.TypeName.Trim();
        var exists = await _dbContext.EquipmentTypes
            .AnyAsync(t => t.VoltageLevelId == request.VoltageLevelId && t.TypeName == name);
        if (exists)
        {
            return ApiResponse<EquipmentTypeDto>.Fail("This equipment type already exists for the selected voltage level.");
        }

        var type = new EquipmentType
        {
            TypeName = name,
            VoltageLevelId = request.VoltageLevelId,
            IsActive = true
        };

        _dbContext.EquipmentTypes.Add(type);
        await _dbContext.SaveChangesAsync();

        type.VoltageLevel = voltageLevel;
        return ApiResponse<EquipmentTypeDto>.Ok(Map(type));
    }

    public async Task<ApiResponse<EquipmentTypeDto>> UpdateAsync(int equipmentTypeId, UpdateEquipmentTypeRequestDto request)
    {
        var type = await _dbContext.EquipmentTypes
            .Include(t => t.VoltageLevel)
            .FirstOrDefaultAsync(t => t.EquipmentTypeId == equipmentTypeId);

        if (type is null)
        {
            return ApiResponse<EquipmentTypeDto>.Fail("Equipment type not found.");
        }

        var name = request.TypeName.Trim();
        var exists = await _dbContext.EquipmentTypes
            .AnyAsync(t => t.EquipmentTypeId != equipmentTypeId && t.VoltageLevelId == type.VoltageLevelId && t.TypeName == name);
        if (exists)
        {
            return ApiResponse<EquipmentTypeDto>.Fail("This equipment type already exists for the selected voltage level.");
        }

        type.TypeName = name;
        type.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<EquipmentTypeDto>.Ok(Map(type));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int equipmentTypeId)
    {
        var type = await _dbContext.EquipmentTypes.FirstOrDefaultAsync(t => t.EquipmentTypeId == equipmentTypeId);
        if (type is null)
        {
            return ApiResponse<object>.Fail("Equipment type not found.");
        }

        var hasEquipment = await _dbContext.Equipment.AnyAsync(e => e.EquipmentTypeId == equipmentTypeId && e.IsActive);
        if (hasEquipment)
        {
            return ApiResponse<object>.Fail("This equipment type has active equipment and cannot be deactivated.");
        }

        type.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static EquipmentTypeDto Map(EquipmentType type)
    {
        return new EquipmentTypeDto
        {
            EquipmentTypeId = type.EquipmentTypeId,
            TypeName = type.TypeName,
            TypeCode = type.TypeCode,
            VoltageLevelId = type.VoltageLevelId,
            VoltageLevelName = type.VoltageLevel?.LevelName,
            IsActive = type.IsActive
        };
    }
}
