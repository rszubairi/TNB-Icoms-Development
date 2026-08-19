using TnbIcoms.Application.Common;
using TnbIcoms.Application.Gnc.Dtos;
using TnbIcoms.Application.Outages.Dtos;

namespace TnbIcoms.Application.Gnc;

public interface IGncService
{
    Task<ApiResponse<List<GncOutageListItemDto>>> ListScheduledAsync(int? zoneId);
    Task<ApiResponse<List<GncOutageListItemDto>>> ListActiveAsync(int? zoneId);
    Task<ApiResponse<List<GncOutageListItemDto>>> ListAuthorisationInForceAsync(int? zoneId);

    Task<ApiResponse<object>> TakeActiveAsync(int outageId, TakeActiveRequestDto request, int currentUserId);
    Task<ApiResponse<object>> CompleteAsync(int outageId, CompleteAuthorisationRequestDto request, int currentUserId);
    Task<ApiResponse<object>> ExtendAsync(int outageId, ExtendAuthorisationRequestDto request, int currentUserId);
    Task<ApiResponse<object>> CloseAsync(int outageId, int currentUserId);
    Task<ApiResponse<object>> NotTakenAsync(int outageId, NotTakenRequestDto request, int currentUserId);
    Task<ApiResponse<object>> CancelAsync(int outageId, CancelOutageRequestDto request, int currentUserId);

    Task<ApiResponse<OutageDetailDto>> CreateForcedOutageAsync(ForcedOutageRequestDto request, int currentUserId);
}
