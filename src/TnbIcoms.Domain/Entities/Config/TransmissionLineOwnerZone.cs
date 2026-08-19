namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// URS Module 1 §5.2.11: zones (beyond the line's default owning zone) whose users may
/// request outages against this line.
/// </summary>
public class TransmissionLineOwnerZone
{
    public int TransmissionLineOwnerZoneId { get; set; }
    public int TransmissionLineId { get; set; }
    public int ZoneId { get; set; }

    public TransmissionLine? TransmissionLine { get; set; }
    public Zone? Zone { get; set; }
}
