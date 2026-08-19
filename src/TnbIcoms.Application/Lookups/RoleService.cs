using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Lookups;

public class RoleService : IRoleService
{
    private readonly AppDbContext _dbContext;

    public RoleService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<RoleLookupDto>>> ListAsync()
    {
        var roles = await _dbContext.AppRoles
            .Where(r => r.IsActive)
            .OrderBy(r => r.RoleName)
            .Select(r => new RoleLookupDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                RoleCode = r.RoleCode,
                IsExternal = r.IsExternal
            })
            .ToListAsync();

        return ApiResponse<List<RoleLookupDto>>.Ok(roles);
    }
}
