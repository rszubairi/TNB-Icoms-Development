using TnbIcoms.Application.Common;
using TnbIcoms.Application.RoleTransferRequests.Dtos;

namespace TnbIcoms.Application.RoleTransferRequests;

public interface IRoleTransferRequestService
{
    Task<ApiResponse<List<RoleTransferRequestDto>>> ListAsync();
    Task<ApiResponse<RoleTransferRequestDto>> CreateAsync(int requestingUserId, CreateRoleTransferRequestDto request);
    Task<ApiResponse<object>> ApproveAsync(int requestId, int approvedByUserId);
    Task<ApiResponse<object>> RejectAsync(int requestId, int approvedByUserId, RejectRoleTransferRequestDto request);
}
