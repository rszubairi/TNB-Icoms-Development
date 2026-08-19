namespace TnbIcoms.Domain.Entities.Outage;

public class ChangeRequest
{
    public int ChangeRequestId { get; set; }
    public int OutageId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? Reason { get; set; }
    public int RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Outage? Outage { get; set; }
}
