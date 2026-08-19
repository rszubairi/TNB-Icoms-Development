using TnbIcoms.Application.ChangeRequests.Dtos;
using TnbIcoms.Application.Common;

namespace TnbIcoms.Application.ChangeRequests;

public interface IChangeRequestService
{
    Task<ApiResponse<List<ChangeRequestBatchDto>>> ListForOutageAsync(int outageId);
    Task<ApiResponse<List<ChangeRequestBatchDto>>> ListPendingAsync();
    Task<ApiResponse<ChangeRequestBatchDto>> CreateAsync(CreateChangeRequestBatchDto request, int requestedByUserId);
    Task<ApiResponse<object>> ApproveAsync(Guid batchId, int reviewerUserId);
    Task<ApiResponse<object>> RejectAsync(Guid batchId, int reviewerUserId, RejectChangeRequestBatchDto request);
}
