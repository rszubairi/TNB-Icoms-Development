using TnbIcoms.Application.Outages.Dtos;

namespace TnbIcoms.Application.Gnc.Dtos;

public class GncOutageListItemDto : OutageListItemDto
{
    public string? AuthorisationNo { get; set; }
    public DateTime? TakenActiveAt { get; set; }
    public DateTime? TakenCompletedAt { get; set; }
    public DateTime? ExtendedTo { get; set; }
    public string? AuthoriserName { get; set; }
    public string TimelineStatus { get; set; } = "OnTime"; // OnTime, Extended, Overdue — only meaningful for Taken-Active rows
}

public class TakeActiveRequestDto
{
    public string AuthorisationNo { get; set; } = string.Empty;
    public int PersonnelId { get; set; }
    public DateTime TakenActiveAt { get; set; }
    public string? Remark { get; set; }
}

public class CompleteAuthorisationRequestDto
{
    public DateTime TakenCompletedAt { get; set; }
    public string? Remark { get; set; }
}

public class ExtendAuthorisationRequestDto
{
    public DateTime ExtendedTo { get; set; }
    public string? Remark { get; set; }
}

public class NotTakenRequestDto
{
    public int NotTakenReasonId { get; set; }
    public string? Remark { get; set; }
}

public class CancelOutageRequestDto
{
    public string? Remark { get; set; }
}

public class ForcedOutageRequestDto
{
    public int StationId { get; set; }
    public int VoltageLevelId { get; set; }
    public int EquipmentTypeId { get; set; }
    public int PrimaryEquipmentId { get; set; }
    public string? LineFilterType { get; set; }
    public List<int> AdditionalEquipmentIds { get; set; } = new();
    public string WorkTypeCode { get; set; } = string.Empty;
    public bool HasPtw { get; set; }
    public DateTime PlannedStartAt { get; set; }
    public DateTime PlannedEndAt { get; set; }
    public int JobTypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int ApprovedById { get; set; }
    public List<OutagePicDto> Pics { get; set; } = new();
}
