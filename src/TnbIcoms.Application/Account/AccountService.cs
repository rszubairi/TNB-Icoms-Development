using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Account.Dtos;
using TnbIcoms.Application.Common;
using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Infrastructure.Identity;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Account;

public class AccountService : IAccountService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<ApiResponse<AccountProfileDto>> GetProfileAsync(int userId)
    {
        var user = await _dbContext.AppUsers
            .Include(u => u.Role)
            .Include(u => u.Zone)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

        if (user is null)
        {
            return ApiResponse<AccountProfileDto>.Fail("Account not found.");
        }

        return ApiResponse<AccountProfileDto>.Ok(Map(user));
    }

    public async Task<ApiResponse<AccountProfileDto>> UpdateProfileAsync(int userId, UpdateAccountProfileRequestDto request)
    {
        var user = await _dbContext.AppUsers
            .Include(u => u.Role)
            .Include(u => u.Zone)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

        if (user is null)
        {
            return ApiResponse<AccountProfileDto>.Fail("Account not found.");
        }

        var email = request.Email.Trim();
        var fullName = request.FullName.Trim();

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
        {
            return ApiResponse<AccountProfileDto>.Fail("Name and email are required.");
        }

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase)
            && await _dbContext.AppUsers.AnyAsync(u => u.UserId != userId && u.Email == email && !u.IsDeleted))
        {
            return ApiResponse<AccountProfileDto>.Fail("This email address is already in use.");
        }

        user.FullName = fullName;
        user.Email = email;
        user.PhoneNumber = request.PhoneNumber?.Trim();

        if (user.AspNetUserId is not null)
        {
            var identityUser = await _userManager.FindByIdAsync(user.AspNetUserId);
            if (identityUser is not null)
            {
                identityUser.FullName = fullName;
                identityUser.PhoneNumber = user.PhoneNumber;
                if (!string.Equals(identityUser.Email, email, StringComparison.OrdinalIgnoreCase))
                {
                    await _userManager.SetEmailAsync(identityUser, email);
                    await _userManager.SetUserNameAsync(identityUser, email);
                }
                else
                {
                    await _userManager.UpdateAsync(identityUser);
                }
            }
        }

        await _dbContext.SaveChangesAsync();

        return ApiResponse<AccountProfileDto>.Ok(Map(user));
    }

    public async Task<ApiResponse<object>> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
        if (user is null)
        {
            return ApiResponse<object>.Fail("Account not found.");
        }

        if (user.AuthType != 2 || user.AspNetUserId is null)
        {
            return ApiResponse<object>.Fail("This account is managed via your TNB Active Directory login. Password changes must go through your Active Directory administrator.");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            return ApiResponse<object>.Fail("New password and confirmation do not match.");
        }

        var identityUser = await _userManager.FindByIdAsync(user.AspNetUserId);
        if (identityUser is null)
        {
            return ApiResponse<object>.Fail("Account not found.");
        }

        var result = await _userManager.ChangePasswordAsync(identityUser, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return ApiResponse<object>.Fail(errors);
        }

        return ApiResponse<object>.Ok(new { });
    }

    private static AccountProfileDto Map(User user)
    {
        return new AccountProfileDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleName = user.Role?.RoleName,
            ZoneName = user.Zone?.ZoneName,
            IsExternal = user.AuthType == 2
        };
    }
}
