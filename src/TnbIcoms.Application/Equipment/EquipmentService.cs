using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Equipment.Dtos;
using TnbIcoms.Infrastructure.Persistence;
using EquipmentEntity = TnbIcoms.Domain.Entities.Config.Equipment;

namespace TnbIcoms.Application.Equipment;

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _dbContext;

    public EquipmentService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<EquipmentListItemDto>>> ListAsync(int? zoneId, int? stationId, int? voltageLevelId, int? equipmentTypeId)
    {
        var query = _dbContext.Equipment
            .Include(e => e.Zone)
            .Include(e => e.Station)
            .Include(e => e.VoltageLevel)
            .Include(e => e.EquipmentType)
            .AsQueryable();

        if (zoneId.HasValue) query = query.Where(e => e.ZoneId == zoneId.Value);
        if (stationId.HasValue) query = query.Where(e => e.StationId == stationId.Value);
        if (voltageLevelId.HasValue) query = query.Where(e => e.VoltageLevelId == voltageLevelId.Value);
        if (equipmentTypeId.HasValue) query = query.Where(e => e.EquipmentTypeId == equipmentTypeId.Value);

        var equipmentList = await query
            .OrderBy(e => e.EquipmentName)
            .ToListAsync();

        var mvaLabels = await GetMvaLabelsAsync(equipmentList.Where(e => e.MvaRatingId.HasValue).Select(e => e.MvaRatingId!.Value));

        var result = equipmentList.Select(e => Map(e, mvaLabels)).ToList();

        return ApiResponse<List<EquipmentListItemDto>>.Ok(result);
    }

    public async Task<ApiResponse<EquipmentListItemDto>> CreateAsync(CreateEquipmentRequestDto request)
    {
        var station = await _dbContext.Stations
            .Include(s => s.Zone)
            .FirstOrDefaultAsync(s => s.StationId == request.StationId && s.IsActive);
        if (station is null)
        {
            return ApiResponse<EquipmentListItemDto>.Fail("Selected station does not exist.");
        }

        var voltageLevel = await _dbContext.VoltageLevels.FirstOrDefaultAsync(v => v.VoltageLevelId == request.VoltageLevelId && v.IsActive);
        if (voltageLevel is null)
        {
            return ApiResponse<EquipmentListItemDto>.Fail("Selected voltage level does not exist.");
        }

        var equipmentType = await _dbContext.EquipmentTypes
            .FirstOrDefaultAsync(t => t.EquipmentTypeId == request.EquipmentTypeId && t.VoltageLevelId == request.VoltageLevelId && t.IsActive);
        if (equipmentType is null)
        {
            return ApiResponse<EquipmentListItemDto>.Fail("Selected equipment type does not belong to the selected voltage level.");
        }

        string? mvaLabel = null;
        if (request.MvaRatingId.HasValue)
        {
            mvaLabel = await _dbContext.DropdownValues
                .Where(d => d.DropdownValueId == request.MvaRatingId.Value && d.IsActive)
                .Select(d => d.ValueLabel)
                .FirstOrDefaultAsync();

            if (mvaLabel is null)
            {
                return ApiResponse<EquipmentListItemDto>.Fail("Selected MVA rating does not exist.");
            }
        }

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return ApiResponse<EquipmentListItemDto>.Fail("Equipment name is required.");
        }

        var (equipmentName, equipmentCode) = BuildNameAndCode(voltageLevel.LevelName, mvaLabel, name);

        if (await _dbContext.Equipment.AnyAsync(e => e.EquipmentCode == equipmentCode))
        {
            return ApiResponse<EquipmentListItemDto>.Fail("An equipment record with this name already exists for this voltage level. Choose a different name.");
        }

        // URS Module 1 §5.2.4: checking the Off-Point box forces the position to Open.
        var isOpen = request.IsOffPoint || request.IsOpen;

        var equipment = new EquipmentEntity
        {
            EquipmentName = equipmentName,
            EquipmentCode = equipmentCode,
            ShortName = name,
            EquipmentTypeId = request.EquipmentTypeId,
            VoltageLevelId = request.VoltageLevelId,
            StationId = request.StationId,
            ZoneId = station.ZoneId,
            MvaRatingId = request.MvaRatingId,
            Position = (byte)(isOpen ? 1 : 0),
            IsOffPoint = request.IsOffPoint,
            OffPointRemark = request.IsOffPoint ? request.OffPointRemark : null,
            IsActive = true
        };

        _dbContext.Equipment.Add(equipment);
        await _dbContext.SaveChangesAsync();

        var mvaLabels = mvaLabel is not null && request.MvaRatingId.HasValue
            ? new Dictionary<int, string> { [request.MvaRatingId.Value] = mvaLabel }
            : new Dictionary<int, string>();

        equipment.Zone = station.Zone;
        equipment.Station = station;
        equipment.VoltageLevel = voltageLevel;
        equipment.EquipmentType = equipmentType;

        return ApiResponse<EquipmentListItemDto>.Ok(Map(equipment, mvaLabels));
    }

    public async Task<ApiResponse<EquipmentListItemDto>> UpdateAsync(int equipmentId, UpdateEquipmentRequestDto request)
    {
        var equipment = await _dbContext.Equipment
            .Include(e => e.Zone)
            .Include(e => e.Station)
            .Include(e => e.VoltageLevel)
            .Include(e => e.EquipmentType)
            .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);

        if (equipment is null)
        {
            return ApiResponse<EquipmentListItemDto>.Fail("Equipment not found.");
        }

        string? mvaLabel = null;
        if (request.MvaRatingId.HasValue)
        {
            mvaLabel = await _dbContext.DropdownValues
                .Where(d => d.DropdownValueId == request.MvaRatingId.Value && d.IsActive)
                .Select(d => d.ValueLabel)
                .FirstOrDefaultAsync();

            if (mvaLabel is null)
            {
                return ApiResponse<EquipmentListItemDto>.Fail("Selected MVA rating does not exist.");
            }
        }

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return ApiResponse<EquipmentListItemDto>.Fail("Equipment name is required.");
        }

        var (equipmentName, equipmentCode) = BuildNameAndCode(equipment.VoltageLevel!.LevelName, mvaLabel, name);

        if (await _dbContext.Equipment.AnyAsync(e => e.EquipmentId != equipmentId && e.EquipmentCode == equipmentCode))
        {
            return ApiResponse<EquipmentListItemDto>.Fail("An equipment record with this name already exists for this voltage level. Choose a different name.");
        }

        var isOpen = request.IsOffPoint || request.IsOpen;

        equipment.EquipmentName = equipmentName;
        equipment.EquipmentCode = equipmentCode;
        equipment.ShortName = name;
        equipment.MvaRatingId = request.MvaRatingId;
        equipment.Position = (byte)(isOpen ? 1 : 0);
        equipment.IsOffPoint = request.IsOffPoint;
        equipment.OffPointRemark = request.IsOffPoint ? request.OffPointRemark : null;
        equipment.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        var mvaLabels = mvaLabel is not null && request.MvaRatingId.HasValue
            ? new Dictionary<int, string> { [request.MvaRatingId.Value] = mvaLabel }
            : new Dictionary<int, string>();

        return ApiResponse<EquipmentListItemDto>.Ok(Map(equipment, mvaLabels));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int equipmentId)
    {
        var equipment = await _dbContext.Equipment.FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);
        if (equipment is null)
        {
            return ApiResponse<object>.Fail("Equipment not found.");
        }

        equipment.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    /// <summary>
    /// URS Module 1 §5.2.4: equipment name is auto-generated as Voltage Level – MVA – Name.
    /// The code is the same string, normalised for uniqueness checks.
    /// </summary>
    private static (string Name, string Code) BuildNameAndCode(string voltageLevelName, string? mvaLabel, string shortName)
    {
        var displayName = mvaLabel is null
            ? $"{voltageLevelName}-{shortName}"
            : $"{voltageLevelName}-{mvaLabel}-{shortName}";

        var code = displayName.Replace(" ", "").ToUpperInvariant();
        return (displayName, code);
    }

    private async Task<Dictionary<int, string>> GetMvaLabelsAsync(IEnumerable<int> mvaRatingIds)
    {
        var ids = mvaRatingIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<int, string>();
        }

        return await _dbContext.DropdownValues
            .Where(d => ids.Contains(d.DropdownValueId))
            .ToDictionaryAsync(d => d.DropdownValueId, d => d.ValueLabel);
    }

    private static EquipmentListItemDto Map(EquipmentEntity equipment, Dictionary<int, string> mvaLabels)
    {
        return new EquipmentListItemDto
        {
            EquipmentId = equipment.EquipmentId,
            EquipmentName = equipment.EquipmentName,
            EquipmentCode = equipment.EquipmentCode,
            ShortName = equipment.ShortName,
            ZoneId = equipment.ZoneId,
            ZoneName = equipment.Zone?.ZoneName,
            StationId = equipment.StationId,
            StationName = equipment.Station?.StationName,
            VoltageLevelId = equipment.VoltageLevelId,
            VoltageLevelName = equipment.VoltageLevel?.LevelName,
            EquipmentTypeId = equipment.EquipmentTypeId,
            EquipmentTypeName = equipment.EquipmentType?.TypeName,
            MvaRatingId = equipment.MvaRatingId,
            MvaRatingLabel = equipment.MvaRatingId.HasValue && mvaLabels.TryGetValue(equipment.MvaRatingId.Value, out var label) ? label : null,
            Position = equipment.Position,
            IsOffPoint = equipment.IsOffPoint,
            OffPointRemark = equipment.OffPointRemark,
            IsActive = equipment.IsActive
        };
    }
}
