namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// URS Module 1 §5.2.13: quad-tower line pairs that must be scheduled for outage together.
/// Selecting either equipment during outage creation surfaces the other as additional
/// equipment.
/// </summary>
public class LinkingLine
{
    public int LinkingLineId { get; set; }
    public int EquipmentId { get; set; }
    public int LinkedEquipmentId { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; } = true;

    public Equipment? Equipment { get; set; }
    public Equipment? LinkedEquipment { get; set; }
}
