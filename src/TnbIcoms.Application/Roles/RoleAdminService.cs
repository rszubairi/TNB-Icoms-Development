using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Roles.Dtos;
using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Roles;

public class RoleAdminService : IRoleAdminService
{
    private readonly AppDbContext _dbContext;

    public RoleAdminService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<RoleListItemDto>>> ListAsync()
    {
        var roles = await _dbContext.AppRoles
            .Include(r => r.Permissions)
            .OrderBy(r => r.RoleName)
            .Select(r => new RoleListItemDto
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName,
                RoleCode = r.RoleCode,
                IsExternal = r.IsExternal,
                IsActive = r.IsActive,
                PermissionCount = r.Permissions.Count(p => p.IsGranted)
            })
            .ToListAsync();

        return ApiResponse<List<RoleListItemDto>>.Ok(roles);
    }

    public async Task<ApiResponse<RoleDetailDto>> GetByIdAsync(int roleId)
    {
        var role = await _dbContext.AppRoles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);

        if (role is null)
        {
            return ApiResponse<RoleDetailDto>.Fail("Role not found.");
        }

        return ApiResponse<RoleDetailDto>.Ok(MapToDetail(role));
    }

    public async Task<ApiResponse<RoleDetailDto>> CreateAsync(CreateRoleRequestDto request)
    {
        var nameExists = await _dbContext.AppRoles.AnyAsync(r => r.RoleName == request.RoleName);
        if (nameExists)
        {
            return ApiResponse<RoleDetailDto>.Fail("A role with this name already exists.");
        }

        var codeExists = await _dbContext.AppRoles.AnyAsync(r => r.RoleCode == request.RoleCode);
        if (codeExists)
        {
            return ApiResponse<RoleDetailDto>.Fail("A role with this code already exists.");
        }

        var role = new Role
        {
            RoleName = request.RoleName,
            RoleCode = request.RoleCode,
            IsExternal = request.IsExternal,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var permission in request.Permissions.Where(p => p.IsGranted))
        {
            role.Permissions.Add(new RolePermission
            {
                ModuleCode = permission.ModuleCode,
                PermissionCode = permission.PermissionCode,
                IsGranted = true
            });
        }

        _dbContext.AppRoles.Add(role);
        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(role.RoleId);
    }

    public async Task<ApiResponse<RoleDetailDto>> UpdateAsync(int roleId, UpdateRoleRequestDto request)
    {
        var role = await _dbContext.AppRoles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.RoleId == roleId);

        if (role is null)
        {
            return ApiResponse<RoleDetailDto>.Fail("Role not found.");
        }

        var nameExists = await _dbContext.AppRoles.AnyAsync(r => r.RoleId != roleId && r.RoleName == request.RoleName);
        if (nameExists)
        {
            return ApiResponse<RoleDetailDto>.Fail("A role with this name already exists.");
        }

        role.RoleName = request.RoleName;
        role.IsExternal = request.IsExternal;
        role.IsActive = request.IsActive;

        _dbContext.RolePermissions.RemoveRange(role.Permissions);
        role.Permissions.Clear();
        foreach (var permission in request.Permissions.Where(p => p.IsGranted))
        {
            role.Permissions.Add(new RolePermission
            {
                RoleId = role.RoleId,
                ModuleCode = permission.ModuleCode,
                PermissionCode = permission.PermissionCode,
                IsGranted = true
            });
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(roleId);
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int roleId)
    {
        var role = await _dbContext.AppRoles.FirstOrDefaultAsync(r => r.RoleId == roleId);
        if (role is null)
        {
            return ApiResponse<object>.Fail("Role not found.");
        }

        var hasActiveUsers = await _dbContext.AppUsers.AnyAsync(u => u.RoleId == roleId && !u.IsDeleted && u.IsActive);
        if (hasActiveUsers)
        {
            return ApiResponse<object>.Fail("This role is assigned to active users and cannot be deactivated.");
        }

        role.IsActive = false;
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static RoleDetailDto MapToDetail(Role role)
    {
        return new RoleDetailDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            RoleCode = role.RoleCode,
            IsExternal = role.IsExternal,
            IsActive = role.IsActive,
            CreatedAt = role.CreatedAt,
            Permissions = role.Permissions
                .Where(p => p.IsGranted)
                .Select(p => new RolePermissionDto
                {
                    ModuleCode = p.ModuleCode,
                    PermissionCode = p.PermissionCode,
                    IsGranted = p.IsGranted
                })
                .ToList()
        };
    }
}
