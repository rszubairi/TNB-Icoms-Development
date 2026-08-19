namespace TnbIcoms.Domain.Entities.Config;

public class OutageTypeRule
{
    public int OutageTypeRuleId { get; set; }
    public string OutageTypeCode { get; set; } = string.Empty; // Planned, Unplanned, Emergency, Forced
    public string WorkTypeCode { get; set; } = string.Empty; // Dead, Live

    public int? MoreThanDays { get; set; }
    public int? MoreThanMonths { get; set; }
    public int? MoreThanYears { get; set; }

    public int? LessThanDays { get; set; }
    public int? LessThanMonths { get; set; }
    public int? LessThanYears { get; set; }

    public string AppliesTo { get; set; } = "ALL"; // "ALL" or comma-separated VoltageLevel names
    public bool IsActive { get; set; } = true;
}
