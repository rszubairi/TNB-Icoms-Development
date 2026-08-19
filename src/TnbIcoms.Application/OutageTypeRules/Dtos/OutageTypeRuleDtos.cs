namespace TnbIcoms.Application.OutageTypeRules.Dtos;

public class OutageTypeRuleDto
{
    public int OutageTypeRuleId { get; set; }
    public string OutageTypeCode { get; set; } = string.Empty;
    public string WorkTypeCode { get; set; } = string.Empty;
    public int? MoreThanDays { get; set; }
    public int? MoreThanMonths { get; set; }
    public int? MoreThanYears { get; set; }
    public int? LessThanDays { get; set; }
    public int? LessThanMonths { get; set; }
    public int? LessThanYears { get; set; }
    public string AppliesTo { get; set; } = "ALL";
    public bool IsActive { get; set; }
}

public class SaveOutageTypeRuleRequestDto
{
    public string OutageTypeCode { get; set; } = string.Empty;
    public string WorkTypeCode { get; set; } = string.Empty;
    public int? MoreThanDays { get; set; }
    public int? MoreThanMonths { get; set; }
    public int? MoreThanYears { get; set; }
    public int? LessThanDays { get; set; }
    public int? LessThanMonths { get; set; }
    public int? LessThanYears { get; set; }
    public string AppliesTo { get; set; } = "ALL";
}
