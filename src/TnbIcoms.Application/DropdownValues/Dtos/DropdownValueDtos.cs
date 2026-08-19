namespace TnbIcoms.Application.DropdownValues.Dtos;

public class DropdownValueDto
{
    public int DropdownValueId { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string ValueCode { get; set; } = string.Empty;
    public string ValueLabel { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDropdownValueRequestDto
{
    public string CategoryCode { get; set; } = string.Empty;
    public string ValueLabel { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
}

public class UpdateDropdownValueRequestDto
{
    public string ValueLabel { get; set; } = string.Empty;
    public string? ParentCode { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ReorderDropdownValueRequestDto
{
    public string Direction { get; set; } = string.Empty; // "up" or "down"
}
