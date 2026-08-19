namespace TnbIcoms.Application.VoltageLevels.Dtos;

public class VoltageLevelDto
{
    public int VoltageLevelId { get; set; }
    public string LevelName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int EquipmentTypeCount { get; set; }
}

public class CreateVoltageLevelRequestDto
{
    public string LevelName { get; set; } = string.Empty;
}

public class UpdateVoltageLevelRequestDto
{
    public string LevelName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
