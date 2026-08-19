namespace TnbIcoms.Application.ConflictingLines.Dtos;

public class ConflictingLineDto
{
    public int ConflictingLineId { get; set; }
    public int EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public int ConflictingEquipmentId { get; set; }
    public string? ConflictingEquipmentName { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; }
}

public class CreateConflictingLineRequestDto
{
    public int EquipmentId { get; set; }
    public int ConflictingEquipmentId { get; set; }
    public string? Remark { get; set; }
}
