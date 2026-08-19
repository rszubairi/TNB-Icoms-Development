namespace TnbIcoms.Application.ChangeRequests.Dtos;

public class ChangeRequestFieldDto
{
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}

public class ChangeRequestBatchDto
{
    public Guid BatchId { get; set; }
    public int OutageId { get; set; }
    public string? OutageNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? RequestedByName { get; set; }
    public DateTime RequestedAt { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }
    public List<ChangeRequestFieldDto> Fields { get; set; } = new();
}

public class CreateChangeRequestBatchDto
{
    public int OutageId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime? NewPlannedStartAt { get; set; }
    public DateTime? NewPlannedEndAt { get; set; }
    public int? NewVoltageLevelId { get; set; }
    public int? NewPrimaryEquipmentId { get; set; }
    public List<int> AddAdditionalEquipmentIds { get; set; } = new();
}

public class RejectChangeRequestBatchDto
{
    public string Comment { get; set; } = string.Empty;
}
