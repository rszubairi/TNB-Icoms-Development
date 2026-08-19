namespace TnbIcoms.Application.EquipmentTypes.Dtos;

public class EquipmentTypeDto
{
    public int EquipmentTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? TypeCode { get; set; }
    public int VoltageLevelId { get; set; }
    public string? VoltageLevelName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateEquipmentTypeRequestDto
{
    public string TypeName { get; set; } = string.Empty;
    public int VoltageLevelId { get; set; }
}

public class UpdateEquipmentTypeRequestDto
{
    public string TypeName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
