using TnbIcoms.Application.Common;
using TnbIcoms.Application.Outages.Dtos;

namespace TnbIcoms.Application.Outages;

public class OutageListFilter
{
    public int? ZoneId { get; set; }
    public string? RequestorStatus { get; set; }
    public string? PlannerStatus { get; set; } // pass "null" (string) to mean "not yet actioned"
    public string? GnmStatus { get; set; }
    public string? OutageCode { get; set; }
    public bool AgreedAndConfirmedOnly { get; set; } // Data Repository: PlannerStatus=Agreed && RequestorStatus=Confirmed
    public bool PendingPlannerReviewOnly { get; set; } // RequestorStatus=Pending && PlannerStatus is null
    public bool PendingConfirmationOnly { get; set; } // PlannerStatus=Agreed && RequestorStatus=Pending
    public bool PendingGnmApprovalOnly { get; set; } // RequestorStatus=Confirmed && PlannerStatus=Agreed && GnmStatus=Pending/Under-Study
    public DateTime? RangeStart { get; set; } // Calendar view: outages overlapping [RangeStart, RangeEnd]
    public DateTime? RangeEnd { get; set; }

    // Customised Reporting (URS §5.9)
    public int? StationId { get; set; }
    public int? JobTypeId { get; set; }
    public string? Keyword { get; set; } // matches OutageNumber or Description
    public DateTime? DateStart { get; set; } // filters on PlannedStartAt >=
    public DateTime? DateEnd { get; set; } // filters on PlannedStartAt <=
    public bool ShowDraft { get; set; } // when false (default), Draft outages are excluded from reports
    public string? SortBy { get; set; } // "date" (default) or "code"
}

public interface IOutageService
{
    Task<ApiResponse<List<OutageListItemDto>>> ListAsync(OutageListFilter filter);
    Task<ApiResponse<OutageDetailDto>> GetByIdAsync(int outageId);
    Task<ApiResponse<OutageDetailDto>> CreateAsync(CreateOutageRequestDto request, int requestorUserId);
    Task<ApiResponse<object>> SubmitDraftAsync(int outageId, int currentUserId);
    Task<ApiResponse<object>> AgreeAsync(int outageId, int currentUserId);
    Task<ApiResponse<object>> DisagreeAsync(int outageId, int currentUserId);
    Task<ApiResponse<object>> ConfirmAsync(int outageId, int currentUserId);
    Task<ApiResponse<object>> RejectAsync(int outageId, int currentUserId);
    Task<ApiResponse<BulkActionResultDto>> BulkAgreeAsync(BulkActionRequestDto request, int currentUserId);
    Task<ApiResponse<BulkActionResultDto>> BulkDisagreeAsync(BulkActionRequestDto request, int currentUserId);
    Task<ApiResponse<BulkActionResultDto>> BulkConfirmAsync(BulkActionRequestDto request, int currentUserId);
    Task<ApiResponse<BulkActionResultDto>> BulkRejectAsync(BulkActionRequestDto request, int currentUserId);
}

public class BulkActionResultDto
{
    public int SucceededCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
