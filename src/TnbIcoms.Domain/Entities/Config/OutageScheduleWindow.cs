namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// URS Module 1 §5.2.9: which calendar months an outage type may be created in,
/// configured separately for Live and Dead work types.
/// </summary>
public class OutageScheduleWindow
{
    public int OutageScheduleWindowId { get; set; }
    public string WorkTypeCode { get; set; } = string.Empty; // Dead, Live
    public string OutageTypeCode { get; set; } = string.Empty; // Planned, Unplanned, Emergency, Forced
    public int Month { get; set; } // 1-12
    public bool IsAllowed { get; set; }
}
