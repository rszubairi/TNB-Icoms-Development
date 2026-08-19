namespace TnbIcoms.Domain.Entities.Config;

public class DropdownValue
{
    public int DropdownValueId { get; set; }
    public string CategoryCode { get; set; } = string.Empty; // e.g. JobType, NotTakenReason
    public string ValueCode { get; set; } = string.Empty;
    public string ValueLabel { get; set; } = string.Empty;
    public string? ParentCode { get; set; } // e.g. JobType -> OutageTypeCode (Planned/Unplanned/Emergency/Forced)
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
