namespace TnbIcoms.Application.Outages.Dtos;

public class OutagePicDto
{
    public string PicName { get; set; } = string.Empty;
    public string PicEmail { get; set; } = string.Empty;
    public string? PicPhone { get; set; }
}

public class OutageListItemDto
{
    public int OutageId { get; set; }
    public string OutageNumber { get; set; } = string.Empty;
    public char OutageCode { get; set; }
    public string OutageTypeCode { get; set; } = string.Empty;
    public string OutageClass { get; set; } = string.Empty;
    public string WorkTypeCode { get; set; } = string.Empty;
    public string? ZoneName { get; set; }
    public string? StationName { get; set; }
    public string? VoltageLevelName { get; set; }
    public string? EquipmentName { get; set; }
    public string? JobTypeLabel { get; set; }
    public string? ProjectName { get; set; }
    public DateTime PlannedStartAt { get; set; }
    public DateTime PlannedEndAt { get; set; }
    public string? RestorationLabel { get; set; }
    public string RequestorStatus { get; set; } = string.Empty;
    public string? PlannerStatus { get; set; }
    public string GnmStatus { get; set; } = string.Empty;
    public string? GncStatus { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> PicNames { get; set; } = new();
    public string? Justification { get; set; }
    public string? Highlights { get; set; }
    public string? Remark { get; set; }
    public string? UnderStudyNotes { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateOutageRequestDto
{
    public int StationId { get; set; }
    public int VoltageLevelId { get; set; }
    public int EquipmentTypeId { get; set; }
    public int PrimaryEquipmentId { get; set; }
    public string? LineFilterType { get; set; }
    public List<int> AdditionalEquipmentIds { get; set; } = new();
    public string WorkTypeCode { get; set; } = string.Empty; // Dead, Live
    public bool HasPtw { get; set; }
    public DateTime PlannedStartAt { get; set; }
    public DateTime PlannedEndAt { get; set; }
    public int? SequenceId { get; set; }
    public int? RestorationId { get; set; }
    public int JobTypeId { get; set; }
    public int? ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<OutagePicDto> Pics { get; set; } = new();
    public bool SubmitNow { get; set; } // false = Save as Draft
}

public class ConflictingOutageDto
{
    public int OutageId { get; set; }
    public string OutageNumber { get; set; } = string.Empty;
    public string? EquipmentName { get; set; }
    public DateTime PlannedStartAt { get; set; }
    public DateTime PlannedEndAt { get; set; }
}

public class OutageDetailDto : OutageListItemDto
{
    public int ZoneId { get; set; }
    public int StationId { get; set; }
    public int VoltageLevelId { get; set; }
    public int PrimaryEquipmentId { get; set; }
    public int EquipmentTypeId { get; set; }
    public List<int> AdditionalEquipmentIds { get; set; } = new();
    public int? SequenceId { get; set; }
    public int? RestorationId { get; set; }
    public int JobTypeId { get; set; }
    public int? ProjectId { get; set; }
    public bool HasPtw { get; set; }
    public List<OutagePicDto> Pics { get; set; } = new();
    public List<ConflictingOutageDto> ConflictingOutages { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

public class BulkActionRequestDto
{
    public List<int> OutageIds { get; set; } = new();
}

public class StudyUpdateRequestDto
{
    public string? Justification { get; set; }
    public string? Highlights { get; set; }
    public string? Remark { get; set; }
    public string? UnderStudyNotes { get; set; }
}
