using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Email;
using TnbIcoms.Application.EmailTemplates;
using TnbIcoms.Application.Users.Dtos;
using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Infrastructure.Identity;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Users;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplateService;

    public UserService(AppDbContext dbContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailTemplateService emailTemplateService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _emailSender = emailSender;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<ApiResponse<PagedResult<UserListItemDto>>> ListAsync(UserListQuery query)
    {
        var users = _dbContext.AppUsers
            .Include(u => u.Role)
            .Include(u => u.Zone)
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        if (query.RoleId.HasValue)
        {
            users = users.Where(u => u.RoleId == query.RoleId.Value);
        }

        if (query.ZoneId.HasValue)
        {
            users = users.Where(u => u.ZoneId == query.ZoneId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            users = users.Where(u => u.FullName.Contains(term) || u.Email.Contains(term) || (u.TnbId != null && u.TnbId.Contains(term)));
        }

        var totalCount = await users.CountAsync();

        var items = await users
            .OrderBy(u => u.FullName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserListItemDto
            {
                UserId = u.UserId,
                TnbId = u.TnbId,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                RoleId = u.RoleId,
                RoleName = u.Role != null ? u.Role.RoleName : null,
                ZoneId = u.ZoneId,
                ZoneName = u.Zone != null ? u.Zone.ZoneName : null,
                IsActive = u.IsActive
            })
            .ToListAsync();

        return ApiResponse<PagedResult<UserListItemDto>>.Ok(new PagedResult<UserListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        });
    }

    public async Task<ApiResponse<UserDetailDto>> GetByIdAsync(int userId)
    {
        var user = await _dbContext.AppUsers
            .Include(u => u.Role)
            .Include(u => u.Zone)
            .Include(u => u.GcuStations)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

        if (user is null)
        {
            return ApiResponse<UserDetailDto>.Fail("User not found.");
        }

        return ApiResponse<UserDetailDto>.Ok(MapToDetail(user));
    }

    public async Task<ApiResponse<UserDetailDto>> CreateAsync(CreateUserRequestDto request, int createdByUserId)
    {
        var existing = await _dbContext.AppUsers.AnyAsync(u => u.Email == request.Email && !u.IsDeleted);
        if (existing)
        {
            return ApiResponse<UserDetailDto>.Fail("A user with this email already exists.");
        }

        var identityUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            TnbId = request.TnbId,
            IsActive = true
        };

        var temporaryPassword = GenerateTemporaryPassword();
        var identityResult = await _userManager.CreateAsync(identityUser, temporaryPassword);
        if (!identityResult.Succeeded)
        {
            var errors = string.Join("; ", identityResult.Errors.Select(e => e.Description));
            return ApiResponse<UserDetailDto>.Fail(errors);
        }

        var domainUser = new User
        {
            TnbId = request.TnbId,
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            AuthType = 2,
            AspNetUserId = identityUser.Id,
            RoleId = request.RoleId,
            ZoneId = request.ZoneId,
            OrganisationId = request.OrganisationId,
            GcuTypeId = request.GcuTypeId,
            IsActive = true,
            CreatedBy = createdByUserId
        };

        foreach (var stationId in request.GcuStationIds.Distinct())
        {
            domainUser.GcuStations.Add(new UserGcuStation { StationId = stationId });
        }

        _dbContext.AppUsers.Add(domainUser);
        await _dbContext.SaveChangesAsync();

        const string templateCode = "UserWelcome";
        var rendered = await _emailTemplateService.RenderAsync(templateCode, new Dictionary<string, string>
        {
            ["FullName"] = request.FullName,
            ["TemporaryPassword"] = temporaryPassword
        });
        if (rendered is not null)
        {
            await _emailSender.SendAsync(request.Email, rendered.Value.Subject, rendered.Value.Body, templateCode);
        }

        return await GetByIdAsync(domainUser.UserId);
    }

    public async Task<ApiResponse<UserDetailDto>> UpdateAsync(int userId, UpdateUserRequestDto request, int updatedByUserId)
    {
        var user = await _dbContext.AppUsers
            .Include(u => u.GcuStations)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);

        if (user is null)
        {
            return ApiResponse<UserDetailDto>.Fail("User not found.");
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        user.RoleId = request.RoleId;
        user.ZoneId = request.ZoneId;
        user.OrganisationId = request.OrganisationId;
        user.GcuTypeId = request.GcuTypeId;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedByUserId;

        var requestedStationIds = request.GcuStationIds.Distinct().ToHashSet();
        foreach (var toRemove in user.GcuStations.Where(gs => !requestedStationIds.Contains(gs.StationId)).ToList())
        {
            user.GcuStations.Remove(toRemove);
        }
        var currentStationIds = user.GcuStations.Select(gs => gs.StationId).ToHashSet();
        foreach (var stationId in requestedStationIds.Except(currentStationIds))
        {
            user.GcuStations.Add(new UserGcuStation { StationId = stationId });
        }

        if (user.AspNetUserId is not null)
        {
            var identityUser = await _userManager.FindByIdAsync(user.AspNetUserId);
            if (identityUser is not null)
            {
                identityUser.FullName = request.FullName;
                identityUser.PhoneNumber = request.PhoneNumber;
                identityUser.IsActive = request.IsActive;
                await _userManager.UpdateAsync(identityUser);
            }
        }

        await _dbContext.SaveChangesAsync();

        return await GetByIdAsync(userId);
    }

    public async Task<ApiResponse<object>> DeactivateAsync(int userId, int updatedByUserId)
    {
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
        if (user is null)
        {
            return ApiResponse<object>.Fail("User not found.");
        }

        user.IsActive = false;
        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = updatedByUserId;

        if (user.AspNetUserId is not null)
        {
            var identityUser = await _userManager.FindByIdAsync(user.AspNetUserId);
            if (identityUser is not null)
            {
                identityUser.IsActive = false;
                await _userManager.UpdateAsync(identityUser);
            }
        }

        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    private static UserDetailDto MapToDetail(User user)
    {
        return new UserDetailDto
        {
            UserId = user.UserId,
            TnbId = user.TnbId,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = user.RoleId,
            RoleName = user.Role?.RoleName,
            ZoneId = user.ZoneId,
            ZoneName = user.Zone?.ZoneName,
            OrganisationId = user.OrganisationId,
            GcuTypeId = user.GcuTypeId,
            GcuStationIds = user.GcuStations.Select(gs => gs.StationId).ToList(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    private static string GenerateTemporaryPassword()
    {
        return $"Tnb{Guid.NewGuid():N}".Substring(0, 12) + "!1";
    }
}
