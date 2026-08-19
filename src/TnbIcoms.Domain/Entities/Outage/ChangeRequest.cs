namespace TnbIcoms.Domain.Entities.Outage;

public class ChangeRequest
{
    public int ChangeRequestId { get; set; }
    public Guid BatchId { get; set; } // Groups the field-level rows from one submission so they are Approved/Rejected as a unit
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
    public string? ReviewComment { get; set; } // GNM's comment, shown to the Requestor on rejection

    public Outage? Outage { get; set; }
}
