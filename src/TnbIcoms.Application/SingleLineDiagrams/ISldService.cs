using TnbIcoms.Application.Common;
using TnbIcoms.Application.SingleLineDiagrams.Dtos;

namespace TnbIcoms.Application.SingleLineDiagrams;

public interface ISldService
{
    Task<ApiResponse<List<SldListItemDto>>> ListAsync(int? stationId, string? status);
    Task<ApiResponse<SldDetailDto>> GetByIdAsync(int id);
    Task<ApiResponse<SldDetailDto>> CreateAsync(CreateSldRequestDto request, int currentUserId);
    Task<ApiResponse<SldDetailDto>> UploadDrawingAsync(int id, Stream content, string originalFileName, int currentUserId);
    Task<ApiResponse<SldDetailDto>> EngineerReviewAsync(int id, EngineerReviewRequestDto request, int currentUserId);
    Task<ApiResponse<SldDetailDto>> SeReviewAsync(int id, StageReviewRequestDto request, int currentUserId);
    Task<ApiResponse<SldDetailDto>> DceReviewAsync(int id, StageReviewRequestDto request, int currentUserId);
    Task<ApiResponse<SldDetailDto>> RequestorApproveAsync(int id, StageReviewRequestDto request, int currentUserId);
    Task<(Stream? Content, string? FileName)> OpenDrawingAsync(int id);
}
