namespace TnbIcoms.Domain.Entities.Config;

public class Equipment
{
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty; // Format: VoltageLevel - MVA - Name
    public string EquipmentCode { get; set; } = string.Empty;
    public int EquipmentTypeId { get; set; }
    public int VoltageLevelId { get; set; }
    public int StationId { get; set; }
    public int ZoneId { get; set; }
    public int? MvaRatingId { get; set; }
    public byte Position { get; set; } // 0 = Closed, 1 = Open
    public bool IsOffPoint { get; set; }
    public string? OffPointRemark { get; set; }
    public string? LineFilterType { get; set; } // Single, Tee-Off, Quad
    public bool IsActive { get; set; } = true;

    public EquipmentType? EquipmentType { get; set; }
    public VoltageLevel? VoltageLevel { get; set; }
    public Station? Station { get; set; }
    public Zone? Zone { get; set; }
}
