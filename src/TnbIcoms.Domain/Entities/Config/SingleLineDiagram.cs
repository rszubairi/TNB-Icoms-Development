namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// URS Module 3 §5.10: station-scoped single line diagram with a multi-stage approval
/// chain (Requestor submit -> GNM Engineer review/assign identity -> S/E -> DCE -> Requestor
/// final approval -> Published). Not tied to a specific outage; outages surface the SLDs for
/// their station in their sidebar by matching StationId.
/// </summary>
public class SingleLineDiagram
{
    public int SingleLineDiagramId { get; set; }
    public int StationId { get; set; }
    public int VoltageLevelId { get; set; }
    public string FlowType { get; set; } = string.Empty; // New, Existing, Update
    public string? Mnemonic { get; set; } // assigned by GNM Engineer at review
    public string? SubstationType { get; set; } // AIS, GIS, Hybrid — assigned by GNM Engineer
    public int? RunningNumber { get; set; } // assigned by GNM Engineer
    public string? DiagramNumber { get; set; } // computed: {voltage}-{SubstationType}-{Mnemonic}-{RunningNumber:D3}
    public string? StoredFileName { get; set; } // drawing, uploaded by GNM Engineer
    public string? OriginalFileName { get; set; }
    public string Status { get; set; } = "PendingEngineerReview";
    // PendingEngineerReview, PendingSE, PendingDCE, PendingRequestorApproval, Published, Rejected
    public string? RejectionReason { get; set; }
    public string? Remark { get; set; }

    public int SubmittedBy { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public int? EngineerReviewedBy { get; set; }
    public DateTime? EngineerReviewedAt { get; set; }
    public int? SeApprovedBy { get; set; }
    public DateTime? SeApprovedAt { get; set; }
    public int? DceApprovedBy { get; set; }
    public DateTime? DceApprovedAt { get; set; }
    public int? RequestorApprovedBy { get; set; }
    public DateTime? RequestorApprovedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    public Station? Station { get; set; }
    public VoltageLevel? VoltageLevel { get; set; }
}
