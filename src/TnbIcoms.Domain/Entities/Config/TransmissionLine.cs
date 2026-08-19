namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// URS Module 1 §5.2.11: the shared setup inputs for a physical line spanning 2-4
/// stations. Creating one generates one Equipment row per involved station (each named
/// from that station's perspective) - see TransmissionLineStation.StationId links.
/// </summary>
public class TransmissionLine
{
    public int TransmissionLineId { get; set; }
    public int VoltageLevelId { get; set; }
    public int EquipmentTypeId { get; set; }
    public int NamingInteger { get; set; }
    public int LineNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public VoltageLevel? VoltageLevel { get; set; }
    public EquipmentType? EquipmentType { get; set; }
    public ICollection<TransmissionLineStation> Stations { get; set; } = new List<TransmissionLineStation>();
    public ICollection<TransmissionLineOwnerZone> OwnerZones { get; set; } = new List<TransmissionLineOwnerZone>();
}
