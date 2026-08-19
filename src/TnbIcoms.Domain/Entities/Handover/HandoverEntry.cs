namespace TnbIcoms.Domain.Entities.Handover;

public class HandoverEntry
{
    public int HandoverEntryId { get; set; }
    public int ShiftId { get; set; }
    public string Category { get; set; } = string.Empty; // Outage, Equipment, General
    public string Description { get; set; } = string.Empty;
    public int? RelatedOutageId { get; set; }
    public bool IsLocked { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public HandoverShift? Shift { get; set; }
}
