using TnbIcoms.Application.Common;
using TnbIcoms.Application.Roles.Dtos;

namespace TnbIcoms.Application.Roles;

public interface IRoleAdminService
{
    Task<ApiResponse<List<RoleListItemDto>>> ListAsync();
    Task<ApiResponse<RoleDetailDto>> GetByIdAsync(int roleId);
    Task<ApiResponse<RoleDetailDto>> CreateAsync(CreateRoleRequestDto request);
    Task<ApiResponse<RoleDetailDto>> UpdateAsync(int roleId, UpdateRoleRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int roleId);
}
