namespace TnbIcoms.Domain.Entities.Config;

public class ConflictingLine
{
    public int ConflictingLineId { get; set; }
    public int EquipmentId { get; set; }
    public int ConflictingEquipmentId { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; } = true;

    public Equipment? Equipment { get; set; }
    public Equipment? ConflictingEquipment { get; set; }
}
