using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Organisations.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Organisations;

public class OrganisationService : IOrganisationService
{
    private readonly AppDbContext _dbContext;

    public OrganisationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<OrganisationListItemDto>>> ListAsync(int? zoneId)
    {
        var query = _dbContext.Organisations
            .Include(o => o.Zone)
            .AsQueryable();

        if (zoneId.HasValue)
        {
            query = query.Where(o => o.ZoneId == zoneId.Value);
        }

        var organisations = await query
            .OrderBy(o => o.OrganisationName)
            .Select(o => Map(o))
            .ToListAsync();

        return ApiResponse<List<OrganisationListItemDto>>.Ok(organisations);
    }

    public async Task<ApiResponse<OrganisationListItemDto>> CreateAsync(CreateOrganisationRequestDto request)
    {
        var zoneExists = await _dbContext.Zones.AnyAsync(z => z.ZoneId == request.ZoneId && z.IsActive);
        if (!zoneExists)
        {
            return ApiResponse<OrganisationListItemDto>.Fail("Selected zone does not exist.");
        }

        var name = request.OrganisationName.Trim();
        var code = request.OrganisationCode.Trim().ToUpperInvariant();

        var duplicateError = await CheckNameAndAbbrConflictAsync(name, code, excludeOrganisationId: null);
        if (duplicateError is not null)
        {
            return ApiResponse<OrganisationListItemDto>.Fail(duplicateError);
        }

        var organisation = new Organisation
        {
            OrganisationName = name,
            OrganisationCode = code,
            ZoneId = request.ZoneId,
            IsGcu = request.IsGcu,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Organisations.Add(organisation);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(organisation).Reference(o => o.Zone).LoadAsync();

        return ApiResponse<OrganisationListItemDto>.Ok(Map(organisation));
    }

    public async Task<ApiResponse<OrganisationListItemDto>> UpdateAsync(int organisationId, UpdateOrganisationRequestDto request)
    {
        var organisation = await _dbContext.Organisations
            .Include(o => o.Zone)
            .FirstOrDefaultAsync(o => o.OrganisationId == organisationId);

        if (organisation is null)
        {
            return ApiResponse<OrganisationListItemDto>.Fail("Organisation not found.");
        }

        var name = request.OrganisationName.Trim();
        var code = request.OrganisationCode.Trim().ToUpperInvariant();

        var duplicateError = await CheckNameAndAbbrConflictAsync(name, code, excludeOrganisationId: organisationId);
        if (duplicateError is not null)
        {
            return ApiResponse<OrganisationListItemDto>.Fail(duplicateError);
        }

        organisation.OrganisationName = name;
        organisation.OrganisationCode = code;
        organisation.IsGcu = request.IsGcu;
        organisation.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<OrganisationListItemDto>.Ok(Map(organisation));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int organisationId)
    {
        var organisation = await _dbContext.Organisations.FirstOrDefaultAsync(o => o.OrganisationId == organisationId);
        if (organisation is null)
        {
            return ApiResponse<object>.Fail("Organisation not found.");
        }

        var hasStations = await _dbContext.Stations.AnyAsync(s => s.OrgId == organisationId && s.IsActive);
        if (hasStations)
        {
            return ApiResponse<object>.Fail("This organisation has active stations and cannot be deactivated.");
        }

        var hasUsers = await _dbContext.AppUsers.AnyAsync(u => u.OrganisationId == organisationId && !u.IsDeleted && u.IsActive);
        if (hasUsers)
        {
            return ApiResponse<object>.Fail("This organisation has active users assigned and cannot be deactivated.");
        }

        organisation.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    /// <summary>
    /// URS Module 1 §5.2.2: Station Name, Organisation Name, and Abbreviations share a single
    /// uniqueness pool across both entity types — no two of any of them may collide.
    /// </summary>
    private async Task<string?> CheckNameAndAbbrConflictAsync(string name, string code, int? excludeOrganisationId)
    {
        var orgNameTaken = await _dbContext.Organisations
            .AnyAsync(o => o.OrganisationName == name && (excludeOrganisationId == null || o.OrganisationId != excludeOrganisationId));
        if (orgNameTaken)
        {
            return "An organisation with this name already exists.";
        }

        var orgCodeTaken = await _dbContext.Organisations
            .AnyAsync(o => o.OrganisationCode == code && (excludeOrganisationId == null || o.OrganisationId != excludeOrganisationId));
        if (orgCodeTaken)
        {
            return "This abbreviation is already in use by another organisation.";
        }

        if (await _dbContext.Stations.AnyAsync(s => s.StationName == name))
        {
            return "This name is already in use by a station.";
        }

        if (await _dbContext.Stations.AnyAsync(s => s.StationAbbr == code))
        {
            return "This abbreviation is already in use by a station.";
        }

        return null;
    }

    private static OrganisationListItemDto Map(Organisation organisation)
    {
        return new OrganisationListItemDto
        {
            OrganisationId = organisation.OrganisationId,
            OrganisationName = organisation.OrganisationName,
            OrganisationCode = organisation.OrganisationCode ?? string.Empty,
            ZoneId = organisation.ZoneId,
            ZoneName = organisation.Zone?.ZoneName,
            IsGcu = organisation.IsGcu,
            IsActive = organisation.IsActive
        };
    }
}
