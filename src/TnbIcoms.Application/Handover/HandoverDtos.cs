namespace TnbIcoms.Application.Handover.Dtos;

public static class HandoverCategories
{
    public static readonly string[] All =
    {
        "Outages",
        "Alarm Checklist",
        "Emergency/Unplanned Outage",
        "System Monitoring",
        "Commissioning",
        "Outage Complete/Off-Point Normalisation",
        "Special Protection Scheme",
        "Special Operations",
        "Other Instructions"
    };
}

public class HandoverEntryDto
{
    public int HandoverEntryId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? RelatedOutageId { get; set; }
    public string? RelatedOutageNumber { get; set; }
    public bool IsLocked { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class HandoverShiftDto
{
    public int ShiftId { get; set; }
    public DateTime ShiftDate { get; set; }
    public string ShiftType { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public int? ControlManagerId { get; set; }
    public string? ControlManagerName { get; set; }
    public int? SwitchEngineer1Id { get; set; }
    public string? SwitchEngineer1Name { get; set; }
    public int? SwitchEngineer2Id { get; set; }
    public string? SwitchEngineer2Name { get; set; }
    public int? DespatcherId { get; set; }
    public string? DespatcherName { get; set; }
    public int? ControlAssistantId { get; set; }
    public string? ControlAssistantName { get; set; }
    public bool IsPassed { get; set; }
    public DateTime? PassedAt { get; set; }
    public string? PassedByName { get; set; }
    public List<HandoverEntryDto> Entries { get; set; } = new();
}

public class HandoverShiftSummaryDto
{
    public int ShiftId { get; set; }
    public DateTime ShiftDate { get; set; }
    public string ShiftType { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public int EntryCount { get; set; }
}

public class UpdateShiftControlRequestDto
{
    public int? ControlManagerId { get; set; }
    public int? SwitchEngineer1Id { get; set; }
    public int? SwitchEngineer2Id { get; set; }
    public int? DespatcherId { get; set; }
    public int? ControlAssistantId { get; set; }
}

public class AddEntryRequestDto
{
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? RelatedOutageId { get; set; }
}
