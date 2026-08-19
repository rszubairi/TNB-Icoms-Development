using TnbIcoms.Application.Common;
using TnbIcoms.Application.LinkingLines.Dtos;

namespace TnbIcoms.Application.LinkingLines;

public interface ILinkingLineService
{
    Task<ApiResponse<List<LinkingLineDto>>> ListAsync();
    Task<ApiResponse<LinkingLineDto>> CreateAsync(CreateLinkingLineRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int linkingLineId);
}
