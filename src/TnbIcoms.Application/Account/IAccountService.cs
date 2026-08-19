using TnbIcoms.Application.Account.Dtos;
using TnbIcoms.Application.Common;

namespace TnbIcoms.Application.Account;

public interface IAccountService
{
    Task<ApiResponse<AccountProfileDto>> GetProfileAsync(int userId);
    Task<ApiResponse<AccountProfileDto>> UpdateProfileAsync(int userId, UpdateAccountProfileRequestDto request);
    Task<ApiResponse<object>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
}
