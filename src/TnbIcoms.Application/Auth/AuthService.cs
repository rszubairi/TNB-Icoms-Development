using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TnbIcoms.Application.Auth.Dtos;
using TnbIcoms.Application.Common;
using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Infrastructure.Identity;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IAdAuthProvider _adAuthProvider;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AppDbContext dbContext,
        IConfiguration configuration,
        IAdAuthProvider adAuthProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _configuration = configuration;
        _adAuthProvider = adAuthProvider;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var identityUser = await _userManager.FindByEmailAsync(request.StaffIdOrEmail)
            ?? await _userManager.Users.FirstOrDefaultAsync(u => u.TnbId == request.StaffIdOrEmail);

        if (identityUser is null || !identityUser.IsActive)
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid credentials or inactive account.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(identityUser, request.Password);
        if (!passwordValid)
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid credentials.");
        }

        var domainUser = await FindDomainUserAsync(identityUser.Id);
        if (domainUser is null)
        {
            return ApiResponse<LoginResponseDto>.Fail("Account is not active.");
        }

        return IssueLoginResponse(identityUser, domainUser);
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginWithAdAsync(LoginRequestDto request)
    {
        var adEnabled = _configuration.GetValue<bool>("Ad:Enabled");
        if (!adEnabled)
        {
            return ApiResponse<LoginResponseDto>.Fail("Corporate AD / SSO sign-in is not enabled.");
        }

        var identityUser = await _userManager.FindByEmailAsync(request.StaffIdOrEmail)
            ?? await _userManager.Users.FirstOrDefaultAsync(u => u.TnbId == request.StaffIdOrEmail);

        if (identityUser is null || !identityUser.IsActive)
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid credentials or inactive account.");
        }

        var domainUser = await FindDomainUserAsync(identityUser.Id);
        if (domainUser is null)
        {
            return ApiResponse<LoginResponseDto>.Fail("Account is not active.");
        }

        if (domainUser.AuthType != 1)
        {
            return ApiResponse<LoginResponseDto>.Fail("This account is not configured for AD sign-in.");
        }

        // Testing-only shortcut: when Ad:BypassEnabled is set, the fixed bypass password
        // authenticates any AD account without contacting the directory or enforcing password
        // strength. Only ever enable this outside production.
        var bypassEnabled = _configuration.GetValue<bool>("Ad:BypassEnabled");
        var bypassPassword = _configuration["Ad:BypassPassword"] ?? "click123";
        var bypassed = bypassEnabled && request.Password == bypassPassword;

        if (!bypassed)
        {
            var adResult = await _adAuthProvider.AuthenticateAsync(identityUser.TnbId ?? request.StaffIdOrEmail, request.Password);
            if (adResult.Status != AdAuthStatus.Success)
            {
                return ApiResponse<LoginResponseDto>.Fail("Invalid AD credentials.");
            }
        }

        return IssueLoginResponse(identityUser, domainUser);
    }

    private async Task<User?> FindDomainUserAsync(string aspNetUserId)
    {
        var domainUser = await _dbContext.AppUsers
            .Include(u => u.Role)
            .Include(u => u.Zone)
            .FirstOrDefaultAsync(u => u.AspNetUserId == aspNetUserId);

        return domainUser is null || !domainUser.IsActive || domainUser.IsDeleted ? null : domainUser;
    }

    private ApiResponse<LoginResponseDto> IssueLoginResponse(ApplicationUser identityUser, User domainUser)
    {
        var bypass2Fa = _configuration.GetValue<bool>("Authentication:Bypass2FA");
        // External (Membership) users require 2FA in production; internal AD users are always bypassed.
        if (!bypass2Fa && domainUser.AuthType == 2)
        {
            return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto { RequiresTwoFactor = true });
        }

        var (accessToken, expiresAt) = GenerateAccessToken(identityUser, domainUser);
        var refreshToken = GenerateRefreshToken();

        return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
        {
            RequiresTwoFactor = false,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserSummaryDto
            {
                UserId = domainUser.UserId,
                FullName = domainUser.FullName,
                Email = domainUser.Email,
                TnbId = domainUser.TnbId,
                RoleId = domainUser.RoleId,
                RoleName = domainUser.Role?.RoleName,
                ZoneId = domainUser.ZoneId,
                ZoneName = domainUser.Zone?.ZoneName
            }
        });
    }

    public Task<ApiResponse<LoginResponseDto>> RefreshAsync(RefreshTokenRequestDto request)
    {
        // TODO: implement persisted refresh-token store/rotation; stubbed as not-yet-supported for MVP.
        return Task.FromResult(ApiResponse<LoginResponseDto>.Fail("Refresh token flow not yet implemented."));
    }

    public Task<ApiResponse<object>> LogoutAsync(string userId)
    {
        return Task.FromResult(ApiResponse<object>.Ok(new { }));
    }

    private (string token, DateTime expiresAt) GenerateAccessToken(ApplicationUser identityUser, User domainUser)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiresAt = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, identityUser.Id),
            new(JwtRegisteredClaimNames.Email, identityUser.Email ?? string.Empty),
            new("userId", domainUser.UserId.ToString()),
            new("roleId", domainUser.RoleId.ToString()),
            new("zoneId", domainUser.ZoneId.ToString()),
            new(ClaimTypes.Name, domainUser.FullName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
