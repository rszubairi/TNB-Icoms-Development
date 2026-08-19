using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.AuthorisationPersonnel.Dtos;
using TnbIcoms.Application.Common;
using TnbIcoms.Infrastructure.Persistence;
using PersonnelEntity = TnbIcoms.Domain.Entities.Config.AuthorisationPersonnel;

namespace TnbIcoms.Application.AuthorisationPersonnel;

public class AuthorisationPersonnelService : IAuthorisationPersonnelService
{
    private readonly AppDbContext _dbContext;

    public AuthorisationPersonnelService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<AuthorisationPersonnelDto>>> ListAsync(int? zoneId)
    {
        var query = _dbContext.AuthorisationPersonnel
            .Include(p => p.Zone)
            .AsQueryable();

        if (zoneId.HasValue)
        {
            query = query.Where(p => p.ZoneId == zoneId.Value);
        }

        var personnel = await query
            .OrderBy(p => p.FullName)
            .Select(p => Map(p))
            .ToListAsync();

        return ApiResponse<List<AuthorisationPersonnelDto>>.Ok(personnel);
    }

    public async Task<ApiResponse<AuthorisationPersonnelDto>> CreateAsync(SaveAuthorisationPersonnelRequestDto request)
    {
        var zoneExists = await _dbContext.Zones.AnyAsync(z => z.ZoneId == request.ZoneId && z.IsActive);
        if (!zoneExists)
        {
            return ApiResponse<AuthorisationPersonnelDto>.Fail("Selected zone does not exist.");
        }

        var personnel = new PersonnelEntity
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            StaffId = string.IsNullOrWhiteSpace(request.StaffId) ? null : request.StaffId.Trim(),
            ZoneId = request.ZoneId,
            Designation = string.IsNullOrWhiteSpace(request.Designation) ? null : request.Designation.Trim(),
            IsActive = true
        };

        _dbContext.AuthorisationPersonnel.Add(personnel);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(personnel).Reference(p => p.Zone).LoadAsync();

        return ApiResponse<AuthorisationPersonnelDto>.Ok(Map(personnel));
    }

    public async Task<ApiResponse<AuthorisationPersonnelDto>> UpdateAsync(int personnelId, SaveAuthorisationPersonnelRequestDto request)
    {
        var personnel = await _dbContext.AuthorisationPersonnel
            .Include(p => p.Zone)
            .FirstOrDefaultAsync(p => p.AuthorisationPersonnelId == personnelId);

        if (personnel is null)
        {
            return ApiResponse<AuthorisationPersonnelDto>.Fail("Personnel record not found.");
        }

        personnel.FullName = request.FullName.Trim();
        personnel.Email = request.Email.Trim();
        personnel.StaffId = string.IsNullOrWhiteSpace(request.StaffId) ? null : request.StaffId.Trim();
        personnel.Designation = string.IsNullOrWhiteSpace(request.Designation) ? null : request.Designation.Trim();
        personnel.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<AuthorisationPersonnelDto>.Ok(Map(personnel));
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int personnelId)
    {
        var personnel = await _dbContext.AuthorisationPersonnel.FirstOrDefaultAsync(p => p.AuthorisationPersonnelId == personnelId);
        if (personnel is null)
        {
            return ApiResponse<object>.Fail("Personnel record not found.");
        }

        personnel.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static AuthorisationPersonnelDto Map(PersonnelEntity personnel)
    {
        return new AuthorisationPersonnelDto
        {
            AuthorisationPersonnelId = personnel.AuthorisationPersonnelId,
            FullName = personnel.FullName,
            Email = personnel.Email,
            StaffId = personnel.StaffId,
            ZoneId = personnel.ZoneId,
            ZoneName = personnel.Zone?.ZoneName,
            Designation = personnel.Designation,
            IsActive = personnel.IsActive
        };
    }
}
