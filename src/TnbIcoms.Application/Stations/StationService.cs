using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Stations.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Stations;

public class StationService : IStationService
{
    private readonly AppDbContext _dbContext;

    public StationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<StationListItemDto>>> ListAsync(int? zoneId, int? orgId)
    {
        var query = _dbContext.Stations
            .Include(s => s.Zone)
            .Include(s => s.Organisation)
            .AsQueryable();

        if (zoneId.HasValue)
        {
            query = query.Where(s => s.ZoneId == zoneId.Value);
        }

        if (orgId.HasValue)
        {
            query = query.Where(s => s.OrgId == orgId.Value);
        }

        var stations = await query
            .OrderBy(s => s.StationName)
            .Select(s => Map(s))
            .ToListAsync();

        return ApiResponse<List<StationListItemDto>>.Ok(stations);
    }

    public async Task<ApiResponse<StationListItemDto>> CreateAsync(CreateStationRequestDto request)
    {
        var zoneExists = await _dbContext.Zones.AnyAsync(z => z.ZoneId == request.ZoneId && z.IsActive);
        if (!zoneExists)
        {
            return ApiResponse<StationListItemDto>.Fail("Selected zone does not exist.");
        }

        var orgInZone = await _dbContext.Organisations
            .AnyAsync(o => o.OrganisationId == request.OrgId && o.ZoneId == request.ZoneId && o.IsActive);
        if (!orgInZone)
        {
            return ApiResponse<StationListItemDto>.Fail("Selected organisation does not belong to this zone.");
        }

        var name = request.StationName.Trim();
        var abbr = request.StationAbbr.Trim().ToUpperInvariant();

        var duplicateError = await CheckNameAndAbbrConflictAsync(name, abbr, excludeStationId: null);
        if (duplicateError is not null)
        {
            return ApiResponse<StationListItemDto>.Fail(duplicateError);
        }

        var station = new Station
        {
            StationName = name,
            StationAbbr = abbr,
            ZoneId = request.ZoneId,
            OrgId = request.OrgId,
            IsActive = true
        };

        _dbContext.Stations.Add(station);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(station).Reference(s => s.Zone).LoadAsync();
        await _dbContext.Entry(station).Reference(s => s.Organisation).LoadAsync();

        return ApiResponse<StationListItemDto>.Ok(Map(station));
    }

    public async Task<ApiResponse<StationListItemDto>> UpdateAsync(int stationId, UpdateStationRequestDto request)
    {
        var station = await _dbContext.Stations
            .Include(s => s.Zone)
            .Include(s => s.Organisation)
            .FirstOrDefaultAsync(s => s.StationId == stationId);

        if (station is null)
        {
            return ApiResponse<StationListItemDto>.Fail("Station not found.");
        }

        var orgInZone = await _dbContext.Organisations
            .AnyAsync(o => o.OrganisationId == request.OrgId && o.ZoneId == station.ZoneId && o.IsActive);
        if (!orgInZone)
        {
            return ApiResponse<StationListItemDto>.Fail("Selected organisation does not belong to this station's zone.");
        }

        var name = request.StationName.Trim();
        var abbr = request.StationAbbr.Trim().ToUpperInvariant();

        var duplicateError = await CheckNameAndAbbrConflictAsync(name, abbr, excludeStationId: stationId);
        if (duplicateError is not null)
        {
            return ApiResponse<StationListItemDto>.Fail(duplicateError);
        }

        station.StationName = name;
        station.StationAbbr = abbr;
        station.OrgId = request.OrgId;
        station.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();
        await _dbContext.Entry(station).Reference(s => s.Organisation).LoadAsync();

        return ApiResponse<StationListItemDto>.Ok(Map(station));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int stationId)
    {
        var station = await _dbContext.Stations.FirstOrDefaultAsync(s => s.StationId == stationId);
        if (station is null)
        {
            return ApiResponse<object>.Fail("Station not found.");
        }

        var hasEquipment = await _dbContext.Equipment.AnyAsync(e => e.StationId == stationId && e.IsActive);
        if (hasEquipment)
        {
            return ApiResponse<object>.Fail("This station has active equipment and cannot be deactivated.");
        }

        station.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    /// <summary>
    /// URS Module 1 §5.2.2: Station Name, Organisation Name, and Abbreviations share a single
    /// uniqueness pool across both entity types — no two of any of them may collide.
    /// </summary>
    private async Task<string?> CheckNameAndAbbrConflictAsync(string name, string abbr, int? excludeStationId)
    {
        var stationNameTaken = await _dbContext.Stations
            .AnyAsync(s => s.StationName == name && (excludeStationId == null || s.StationId != excludeStationId));
        if (stationNameTaken)
        {
            return "A station with this name already exists.";
        }

        var stationAbbrTaken = await _dbContext.Stations
            .AnyAsync(s => s.StationAbbr == abbr && (excludeStationId == null || s.StationId != excludeStationId));
        if (stationAbbrTaken)
        {
            return "This abbreviation is already in use by another station.";
        }

        if (await _dbContext.Organisations.AnyAsync(o => o.OrganisationName == name))
        {
            return "This name is already in use by an organisation.";
        }

        if (await _dbContext.Organisations.AnyAsync(o => o.OrganisationCode == abbr))
        {
            return "This abbreviation is already in use by an organisation.";
        }

        return null;
    }

    private static StationListItemDto Map(Station station)
    {
        return new StationListItemDto
        {
            StationId = station.StationId,
            StationName = station.StationName,
            StationAbbr = station.StationAbbr,
            ZoneId = station.ZoneId,
            ZoneName = station.Zone?.ZoneName,
            OrgId = station.OrgId,
            OrganisationName = station.Organisation?.OrganisationName,
            SldFileUrl = station.SldFileUrl,
            IsActive = station.IsActive
        };
    }
}
