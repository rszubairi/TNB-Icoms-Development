using TnbIcoms.Application.Auth.Dtos;
using TnbIcoms.Application.Common;

namespace TnbIcoms.Application.Auth;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<LoginResponseDto>> RefreshAsync(RefreshTokenRequestDto request);
    Task<ApiResponse<object>> LogoutAsync(string userId);
}
