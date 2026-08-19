namespace TnbIcoms.Application.SingleLineDiagrams.Dtos;

public class SldListItemDto
{
    public int SingleLineDiagramId { get; set; }
    public int StationId { get; set; }
    public string? StationName { get; set; }
    public string? VoltageLevelName { get; set; }
    public string FlowType { get; set; } = string.Empty;
    public string? Mnemonic { get; set; }
    public string? SubstationType { get; set; }
    public string? DiagramNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool HasDrawing { get; set; }
    public string SubmittedByName { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

public class SldDetailDto : SldListItemDto
{
    public int VoltageLevelId { get; set; }
    public string? RejectionReason { get; set; }
    public string? Remark { get; set; }
    public string? EngineerReviewedByName { get; set; }
    public DateTime? EngineerReviewedAt { get; set; }
    public string? SeApprovedByName { get; set; }
    public DateTime? SeApprovedAt { get; set; }
    public string? DceApprovedByName { get; set; }
    public DateTime? DceApprovedAt { get; set; }
    public string? RequestorApprovedByName { get; set; }
    public DateTime? RequestorApprovedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class CreateSldRequestDto
{
    public int StationId { get; set; }
    public int VoltageLevelId { get; set; }
    public string FlowType { get; set; } = string.Empty; // New, Existing, Update
    public string? Remark { get; set; }
}

public class EngineerReviewRequestDto
{
    public bool Approve { get; set; }
    public string? Mnemonic { get; set; }
    public string? SubstationType { get; set; } // AIS, GIS, Hybrid
    public string? RejectionReason { get; set; }
}

public class StageReviewRequestDto
{
    public bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}
