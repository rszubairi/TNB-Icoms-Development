namespace TnbIcoms.Domain.Entities.Config;

public class VoltageLevel
{
    public int VoltageLevelId { get; set; }
    public string LevelName { get; set; } = string.Empty; // e.g. 500kV, 275kV, 132kV, 33kV
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
