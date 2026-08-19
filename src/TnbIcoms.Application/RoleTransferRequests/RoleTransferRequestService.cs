using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Email;
using TnbIcoms.Application.EmailTemplates;
using TnbIcoms.Application.RoleTransferRequests.Dtos;
using TnbIcoms.Domain.Entities.Auth;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.RoleTransferRequests;

public class RoleTransferRequestService : IRoleTransferRequestService
{
    private readonly AppDbContext _dbContext;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplateService;

    public RoleTransferRequestService(AppDbContext dbContext, IEmailSender emailSender, IEmailTemplateService emailTemplateService)
    {
        _dbContext = dbContext;
        _emailSender = emailSender;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<ApiResponse<List<RoleTransferRequestDto>>> ListAsync()
    {
        var requests = await _dbContext.RoleTransferRequests
            .Include(r => r.User)
            .Include(r => r.FromRole)
            .Include(r => r.ToRole)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();

        var zoneIds = requests.SelectMany(r => new[] { r.FromZoneId, r.ToZoneId })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var zoneNames = await _dbContext.Zones
            .Where(z => zoneIds.Contains(z.ZoneId))
            .ToDictionaryAsync(z => z.ZoneId, z => z.ZoneName);

        var dtos = requests.Select(r => Map(r, zoneNames)).ToList();

        return ApiResponse<List<RoleTransferRequestDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<RoleTransferRequestDto>> CreateAsync(int requestingUserId, CreateRoleTransferRequestDto request)
    {
        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserId == requestingUserId && !u.IsDeleted);
        if (user is null)
        {
            return ApiResponse<RoleTransferRequestDto>.Fail("Account not found.");
        }

        if (!request.RequestedRoleId.HasValue && !request.RequestedZoneId.HasValue)
        {
            return ApiResponse<RoleTransferRequestDto>.Fail("Select a new role, a new zone, or both.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            return ApiResponse<RoleTransferRequestDto>.Fail("A request summary is required.");
        }

        var hasPending = await _dbContext.RoleTransferRequests
            .AnyAsync(r => r.UserId == requestingUserId && r.Status == "Pending");
        if (hasPending)
        {
            return ApiResponse<RoleTransferRequestDto>.Fail("You already have a pending role/zone change request.");
        }

        var transferRequest = new RoleTransferRequest
        {
            UserId = requestingUserId,
            FromRoleId = user.RoleId,
            ToRoleId = request.RequestedRoleId,
            FromZoneId = user.ZoneId,
            ToZoneId = request.RequestedZoneId,
            Reason = request.Reason.Trim(),
            Status = "Pending",
            RequestedAt = DateTime.UtcNow,
            RequestedBy = requestingUserId
        };

        _dbContext.RoleTransferRequests.Add(transferRequest);
        await _dbContext.SaveChangesAsync();

        await NotifySysAdminsAsync(user, request);

        var listResult = await ListAsync();
        var dto = listResult.Data?.FirstOrDefault(d => d.Id == transferRequest.RoleTransferRequestId);

        return dto is not null
            ? ApiResponse<RoleTransferRequestDto>.Ok(dto)
            : ApiResponse<RoleTransferRequestDto>.Fail("Request created but could not be reloaded.");
    }

    public async Task<ApiResponse<object>> ApproveAsync(int requestId, int approvedByUserId)
    {
        var transferRequest = await _dbContext.RoleTransferRequests.FirstOrDefaultAsync(r => r.RoleTransferRequestId == requestId);
        if (transferRequest is null)
        {
            return ApiResponse<object>.Fail("Request not found.");
        }

        if (transferRequest.Status != "Pending")
        {
            return ApiResponse<object>.Fail("This request has already been actioned.");
        }

        var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserId == transferRequest.UserId && !u.IsDeleted);
        if (user is null)
        {
            return ApiResponse<object>.Fail("The requesting user no longer exists.");
        }

        if (transferRequest.ToRoleId.HasValue)
        {
            user.RoleId = transferRequest.ToRoleId.Value;
        }
        if (transferRequest.ToZoneId.HasValue)
        {
            user.ZoneId = transferRequest.ToZoneId.Value;
        }
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = approvedByUserId;

        transferRequest.Status = "Approved";
        transferRequest.ApprovedBy = approvedByUserId;
        transferRequest.ApprovedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public async Task<ApiResponse<object>> RejectAsync(int requestId, int approvedByUserId, RejectRoleTransferRequestDto request)
    {
        var transferRequest = await _dbContext.RoleTransferRequests.FirstOrDefaultAsync(r => r.RoleTransferRequestId == requestId);
        if (transferRequest is null)
        {
            return ApiResponse<object>.Fail("Request not found.");
        }

        if (transferRequest.Status != "Pending")
        {
            return ApiResponse<object>.Fail("This request has already been actioned.");
        }

        transferRequest.Status = "Rejected";
        transferRequest.ApprovedBy = approvedByUserId;
        transferRequest.ApprovedAt = DateTime.UtcNow;
        transferRequest.RejectionReason = request.Reason;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    /// <summary>
    /// URS Module 1 §5.3: submitting a role/zone change request emails the SysAdmin
    /// with the requested change and the user's justification.
    /// </summary>
    private async Task NotifySysAdminsAsync(User requestingUser, CreateRoleTransferRequestDto request)
    {
        var sysAdminEmails = await _dbContext.AppUsers
            .Include(u => u.Role)
            .Where(u => u.IsActive && !u.IsDeleted && u.Role != null && u.Role.RoleCode == "SYSADMIN")
            .Select(u => u.Email)
            .ToListAsync();

        if (sysAdminEmails.Count == 0)
        {
            return;
        }

        var newRole = request.RequestedRoleId.HasValue
            ? (await _dbContext.AppRoles.FindAsync(request.RequestedRoleId.Value))?.RoleName
            : null;
        var newZone = request.RequestedZoneId.HasValue
            ? (await _dbContext.Zones.FindAsync(request.RequestedZoneId.Value))?.ZoneName
            : null;

        const string templateCode = "RoleTransferRequestSubmitted";
        var rendered = await _emailTemplateService.RenderAsync(templateCode, new Dictionary<string, string>
        {
            ["RequestingUserFullName"] = requestingUser.FullName,
            ["NewRole"] = newRole ?? "(unchanged)",
            ["NewZone"] = newZone ?? "(unchanged)",
            ["Reason"] = System.Net.WebUtility.HtmlEncode(request.Reason)
        });
        if (rendered is null) return;

        foreach (var email in sysAdminEmails)
        {
            await _emailSender.SendAsync(email, rendered.Value.Subject, rendered.Value.Body, templateCode);
        }
    }

    private static RoleTransferRequestDto Map(RoleTransferRequest request, Dictionary<int, string> zoneNames)
    {
        return new RoleTransferRequestDto
        {
            Id = request.RoleTransferRequestId,
            TnbId = request.User?.TnbId ?? string.Empty,
            FullName = request.User?.FullName ?? string.Empty,
            CurrentRoleName = request.FromRole?.RoleName,
            RequestedRoleName = request.ToRole?.RoleName,
            CurrentZoneName = request.FromZoneId.HasValue && zoneNames.TryGetValue(request.FromZoneId.Value, out var fromZone) ? fromZone : null,
            RequestedZoneName = request.ToZoneId.HasValue && zoneNames.TryGetValue(request.ToZoneId.Value, out var toZone) ? toZone : null,
            Reason = request.Reason,
            RequestedAt = request.RequestedAt,
            Status = request.Status,
            RejectionReason = request.RejectionReason
        };
    }
}
