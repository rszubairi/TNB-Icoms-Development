namespace TnbIcoms.Domain.Entities.Config;

public class EquipmentType
{
    public int EquipmentTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty; // Transformer, Circuit Breaker, Line, Busbar
    public string? TypeCode { get; set; }
    public int VoltageLevelId { get; set; }
    public bool IsActive { get; set; } = true;

    public VoltageLevel? VoltageLevel { get; set; }
}
