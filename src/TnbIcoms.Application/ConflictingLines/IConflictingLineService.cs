using TnbIcoms.Application.Common;
using TnbIcoms.Application.ConflictingLines.Dtos;

namespace TnbIcoms.Application.ConflictingLines;

public interface IConflictingLineService
{
    Task<ApiResponse<List<ConflictingLineDto>>> ListAsync();
    Task<ApiResponse<ConflictingLineDto>> CreateAsync(CreateConflictingLineRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int conflictingLineId);
}
