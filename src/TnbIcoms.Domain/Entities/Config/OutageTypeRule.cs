namespace TnbIcoms.Domain.Entities.Config;

public class OutageTypeRule
{
    public int OutageTypeRuleId { get; set; }
    public string OutageTypeCode { get; set; } = string.Empty; // Planned, Unplanned, Emergency, Forced
    public int? MinLeadDays { get; set; }
    public int? MaxLeadDays { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
