using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.TransmissionLines.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;
using EquipmentEntity = TnbIcoms.Domain.Entities.Config.Equipment;

namespace TnbIcoms.Application.TransmissionLines;

public class TransmissionLineService : ITransmissionLineService
{
    private readonly AppDbContext _dbContext;

    public TransmissionLineService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<TransmissionLineDto>>> ListAsync()
    {
        var lines = await _dbContext.TransmissionLines
            .Include(l => l.VoltageLevel)
            .Include(l => l.EquipmentType)
            .Include(l => l.Stations).ThenInclude(s => s.Station)
            .Include(l => l.Stations).ThenInclude(s => s.GeneratedEquipment)
            .Include(l => l.OwnerZones).ThenInclude(oz => oz.Zone)
            .OrderByDescending(l => l.TransmissionLineId)
            .ToListAsync();

        return ApiResponse<List<TransmissionLineDto>>.Ok(lines.Select(Map).ToList());
    }

    public async Task<ApiResponse<List<GeneratedNameDto>>> PreviewAsync(TransmissionLineRequestDto request)
    {
        var (error, stations, _, _) = await ValidateAndLoadAsync(request);
        if (error is not null)
        {
            return ApiResponse<List<GeneratedNameDto>>.Fail(error);
        }

        return ApiResponse<List<GeneratedNameDto>>.Ok(BuildGeneratedNames(stations!, request.NamingInteger, request.LineNumber));
    }

    public async Task<ApiResponse<TransmissionLineDto>> CreateAsync(TransmissionLineRequestDto request)
    {
        var (error, stations, voltageLevel, equipmentType) = await ValidateAndLoadAsync(request);
        if (error is not null)
        {
            return ApiResponse<TransmissionLineDto>.Fail(error);
        }

        var generatedNames = BuildGeneratedNames(stations!, request.NamingInteger, request.LineNumber);
        var codes = generatedNames.Select(n => n.GeneratedName.Replace(" ", "").ToUpperInvariant()).ToList();

        if (await _dbContext.Equipment.AnyAsync(e => codes.Contains(e.EquipmentCode)))
        {
            return ApiResponse<TransmissionLineDto>.Fail("One or more generated line names already exist. Choose a different Naming Integer or Line Number.");
        }

        var line = new TransmissionLine
        {
            VoltageLevelId = request.VoltageLevelId,
            EquipmentTypeId = request.EquipmentTypeId,
            NamingInteger = request.NamingInteger,
            LineNumber = request.LineNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.TransmissionLines.Add(line);

        var lineFilterType = LineNamingCalculator.LineFilterTypeFor(stations!.Count);

        for (var i = 0; i < stations.Count; i++)
        {
            var stationEntity = stations[i].Entity;
            var generatedName = generatedNames[i].GeneratedName;
            var code = generatedName.Replace(" ", "").ToUpperInvariant();

            var equipment = new EquipmentEntity
            {
                EquipmentName = generatedName,
                EquipmentCode = code,
                ShortName = generatedName,
                EquipmentTypeId = request.EquipmentTypeId,
                VoltageLevelId = request.VoltageLevelId,
                StationId = stationEntity.StationId,
                ZoneId = stationEntity.ZoneId,
                LineFilterType = lineFilterType,
                Position = 0,
                IsActive = true,
                TransmissionLine = line
            };
            _dbContext.Equipment.Add(equipment);

            line.Stations.Add(new TransmissionLineStation
            {
                TransmissionLine = line,
                StationId = stationEntity.StationId,
                SequenceOrder = i,
                GeneratedEquipment = equipment
            });
        }

        // Default owner zone: the first station's zone (URS §5.2.11).
        line.OwnerZones.Add(new TransmissionLineOwnerZone { TransmissionLine = line, ZoneId = stations[0].Entity.ZoneId });

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(line.TransmissionLineId);
    }

    public async Task<ApiResponse<TransmissionLineDto>> AddOwnerZoneAsync(int transmissionLineId, AddOwnerZoneRequestDto request)
    {
        var line = await _dbContext.TransmissionLines.FirstOrDefaultAsync(l => l.TransmissionLineId == transmissionLineId);
        if (line is null)
        {
            return ApiResponse<TransmissionLineDto>.Fail("Line not found.");
        }

        var zoneExists = await _dbContext.Zones.AnyAsync(z => z.ZoneId == request.ZoneId && z.IsActive);
        if (!zoneExists)
        {
            return ApiResponse<TransmissionLineDto>.Fail("Selected zone does not exist.");
        }

        var alreadyOwner = await _dbContext.TransmissionLineOwnerZones
            .AnyAsync(oz => oz.TransmissionLineId == transmissionLineId && oz.ZoneId == request.ZoneId);
        if (!alreadyOwner)
        {
            _dbContext.TransmissionLineOwnerZones.Add(new TransmissionLineOwnerZone
            {
                TransmissionLineId = transmissionLineId,
                ZoneId = request.ZoneId
            });
            await _dbContext.SaveChangesAsync();
        }

        return await GetByIdAsync(transmissionLineId);
    }

    public async Task<ApiResponse<object>> RemoveOwnerZoneAsync(int transmissionLineId, int zoneId)
    {
        var ownerZone = await _dbContext.TransmissionLineOwnerZones
            .FirstOrDefaultAsync(oz => oz.TransmissionLineId == transmissionLineId && oz.ZoneId == zoneId);

        if (ownerZone is null)
        {
            return ApiResponse<object>.Fail("This zone is not an owner of the line.");
        }

        var remainingCount = await _dbContext.TransmissionLineOwnerZones.CountAsync(oz => oz.TransmissionLineId == transmissionLineId);
        if (remainingCount <= 1)
        {
            return ApiResponse<object>.Fail("A line must keep at least one owner zone.");
        }

        _dbContext.TransmissionLineOwnerZones.Remove(ownerZone);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int transmissionLineId)
    {
        var line = await _dbContext.TransmissionLines
            .Include(l => l.Stations).ThenInclude(s => s.GeneratedEquipment)
            .FirstOrDefaultAsync(l => l.TransmissionLineId == transmissionLineId);

        if (line is null)
        {
            return ApiResponse<object>.Fail("Line not found.");
        }

        line.IsActive = false;
        foreach (var station in line.Stations)
        {
            if (station.GeneratedEquipment is not null)
            {
                station.GeneratedEquipment.IsActive = false;
            }
        }

        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private async Task<ApiResponse<TransmissionLineDto>> GetByIdAsync(int transmissionLineId)
    {
        var line = await _dbContext.TransmissionLines
            .Include(l => l.VoltageLevel)
            .Include(l => l.EquipmentType)
            .Include(l => l.Stations).ThenInclude(s => s.Station)
            .Include(l => l.Stations).ThenInclude(s => s.GeneratedEquipment)
            .Include(l => l.OwnerZones).ThenInclude(oz => oz.Zone)
            .FirstOrDefaultAsync(l => l.TransmissionLineId == transmissionLineId);

        return line is null
            ? ApiResponse<TransmissionLineDto>.Fail("Line not found.")
            : ApiResponse<TransmissionLineDto>.Ok(Map(line));
    }

    private List<GeneratedNameDto> BuildGeneratedNames(List<(Station Entity, StationRef Ref)> stations, int namingInteger, int lineNumber)
    {
        var chain = stations.Select(s => s.Ref).ToList();

        return stations.Select(s => new GeneratedNameDto
        {
            StationId = s.Entity.StationId,
            StationAbbr = s.Entity.StationAbbr,
            GeneratedName = LineNamingCalculator.GenerateName(s.Ref, chain, namingInteger, lineNumber)
        }).ToList();
    }

    private async Task<(string? Error, List<(Station Entity, StationRef Ref)>? Stations, VoltageLevel? Voltage, EquipmentType? Type)> ValidateAndLoadAsync(TransmissionLineRequestDto request)
    {
        if (request.StationIdsInOrder.Count is < 2 or > 4)
        {
            return ("A line needs between 2 and 4 stations.", null, null, null);
        }

        if (request.StationIdsInOrder.Distinct().Count() != request.StationIdsInOrder.Count)
        {
            return ("Each station may only appear once in a line.", null, null, null);
        }

        if (request.NamingInteger <= 0 || request.LineNumber <= 0)
        {
            return ("Naming Integer and Line Number must be positive.", null, null, null);
        }

        var voltageLevel = await _dbContext.VoltageLevels.FirstOrDefaultAsync(v => v.VoltageLevelId == request.VoltageLevelId && v.IsActive);
        if (voltageLevel is null)
        {
            return ("Selected voltage level does not exist.", null, null, null);
        }

        var equipmentType = await _dbContext.EquipmentTypes
            .FirstOrDefaultAsync(t => t.EquipmentTypeId == request.EquipmentTypeId && t.VoltageLevelId == request.VoltageLevelId && t.IsActive);
        if (equipmentType is null)
        {
            return ("Selected equipment type does not belong to the selected voltage level.", null, null, null);
        }

        var stationEntities = await _dbContext.Stations
            .Where(s => request.StationIdsInOrder.Contains(s.StationId) && s.IsActive)
            .ToListAsync();

        if (stationEntities.Count != request.StationIdsInOrder.Count)
        {
            return ("One or more selected stations do not exist.", null, null, null);
        }

        var orderedStations = request.StationIdsInOrder
            .Select(id => stationEntities.First(s => s.StationId == id))
            .Select(s => (Entity: s, Ref: new StationRef(s.StationId, s.StationAbbr)))
            .ToList();

        return (null, orderedStations, voltageLevel, equipmentType);
    }

    private static TransmissionLineDto Map(TransmissionLine line)
    {
        var orderedStations = line.Stations.OrderBy(s => s.SequenceOrder).ToList();

        return new TransmissionLineDto
        {
            TransmissionLineId = line.TransmissionLineId,
            VoltageLevelId = line.VoltageLevelId,
            VoltageLevelName = line.VoltageLevel?.LevelName,
            EquipmentTypeId = line.EquipmentTypeId,
            EquipmentTypeName = line.EquipmentType?.TypeName,
            NamingInteger = line.NamingInteger,
            LineNumber = line.LineNumber,
            LineFilterType = LineNamingCalculator.LineFilterTypeFor(orderedStations.Count),
            IsActive = line.IsActive,
            GeneratedNames = orderedStations.Select(s => new GeneratedNameDto
            {
                StationId = s.StationId,
                StationAbbr = s.Station?.StationAbbr ?? string.Empty,
                GeneratedName = s.GeneratedEquipment?.EquipmentName ?? string.Empty
            }).ToList(),
            OwnerZoneIds = line.OwnerZones.Select(oz => oz.ZoneId).ToList(),
            OwnerZoneNames = line.OwnerZones.Select(oz => oz.Zone?.ZoneName ?? string.Empty).ToList()
        };
    }
}
