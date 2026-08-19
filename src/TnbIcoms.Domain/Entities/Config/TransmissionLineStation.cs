namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// One station in a TransmissionLine's chain, in order (0 = first end, last index = other
/// end; anything between is a Tee-Off position). SequenceOrder drives the naming convention.
/// </summary>
public class TransmissionLineStation
{
    public int TransmissionLineStationId { get; set; }
    public int TransmissionLineId { get; set; }
    public int StationId { get; set; }
    public int SequenceOrder { get; set; }
    public int? GeneratedEquipmentId { get; set; } // The Equipment row generated for this station's perspective

    public TransmissionLine? TransmissionLine { get; set; }
    public Station? Station { get; set; }
    public Equipment? GeneratedEquipment { get; set; }
}
