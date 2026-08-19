using TnbIcoms.Application.Common;
using TnbIcoms.Application.Users.Dtos;

namespace TnbIcoms.Application.Users;

public class UserListQuery
{
    public int? RoleId { get; set; }
    public int? ZoneId { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public interface IUserService
{
    Task<ApiResponse<PagedResult<UserListItemDto>>> ListAsync(UserListQuery query);
    Task<ApiResponse<UserDetailDto>> GetByIdAsync(int userId);
    Task<ApiResponse<UserDetailDto>> CreateAsync(CreateUserRequestDto request, int createdByUserId);
    Task<ApiResponse<UserDetailDto>> UpdateAsync(int userId, UpdateUserRequestDto request, int updatedByUserId);
    Task<ApiResponse<object>> DeactivateAsync(int userId, int updatedByUserId);
}
