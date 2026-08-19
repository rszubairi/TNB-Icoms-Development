using TnbIcoms.Application.Common;
using TnbIcoms.Application.CommissioningMemos.Dtos;

namespace TnbIcoms.Application.CommissioningMemos;

public interface ICommissioningMemoService
{
    Task<ApiResponse<List<CommissioningMemoListItemDto>>> ListAsync(int? outageId, string? status);
    Task<ApiResponse<CommissioningMemoDetailDto>> GetByIdAsync(int id);
    Task<ApiResponse<CommissioningMemoDetailDto>> CreateAsync(CreateCommissioningMemoRequestDto request, int currentUserId);
    Task<ApiResponse<CommissioningMemoDetailDto>> EngineerPicReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId);
    Task<ApiResponse<CommissioningMemoDetailDto>> SeReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId);
    Task<ApiResponse<CommissioningMemoDetailDto>> DceReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId);
    Task<ApiResponse<CommissioningMemoDetailDto>> CeGnmReviewAsync(int id, MemoStageReviewRequestDto request, int currentUserId);
    Task<ApiResponse<CommissioningMemoDetailDto>> FinalSignOffAsync(int id, MemoStageReviewRequestDto request, int currentUserId);
    Task<ApiResponse<CommissioningMemoDetailDto>> SetCommissioningResultAsync(int id, SetCommissioningResultRequestDto request, int currentUserId);
    Task<byte[]> GenerateCoverPagePdfAsync(int id);
}
