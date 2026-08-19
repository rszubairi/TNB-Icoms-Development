using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Domain.Entities.Handover;

public class HandoverShift
{
    public int ShiftId { get; set; }
    public DateTime ShiftDate { get; set; }
    public string ShiftType { get; set; } = string.Empty; // Morning, Evening, Night
    public int ZoneId { get; set; }
    public int? ControlManagerId { get; set; }
    public int? SwitchEngineer1Id { get; set; }
    public int? SwitchEngineer2Id { get; set; }
    public int? DespatcherId { get; set; }
    public int? ControlAssistantId { get; set; }
    public bool IsPassed { get; set; }
    public DateTime? PassedAt { get; set; }
    public int? PassedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Zone? Zone { get; set; }
    public ICollection<HandoverEntry> Entries { get; set; } = new List<HandoverEntry>();
}
