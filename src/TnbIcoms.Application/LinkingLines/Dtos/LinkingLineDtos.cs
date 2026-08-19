namespace TnbIcoms.Application.LinkingLines.Dtos;

public class LinkingLineDto
{
    public int LinkingLineId { get; set; }
    public int EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public int LinkedEquipmentId { get; set; }
    public string? LinkedEquipmentName { get; set; }
    public string? Remark { get; set; }
    public bool IsActive { get; set; }
}

public class CreateLinkingLineRequestDto
{
    public int EquipmentId { get; set; }
    public int LinkedEquipmentId { get; set; }
    public string? Remark { get; set; }
}
