namespace TnbIcoms.Domain.Entities.Outage;

public class CommissioningMemo
{
    public int CommissioningMemoId { get; set; }
    public int OutageId { get; set; }
    public string MemoNo { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty; // structured document JSON/HTML
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Outage? Outage { get; set; }
}
