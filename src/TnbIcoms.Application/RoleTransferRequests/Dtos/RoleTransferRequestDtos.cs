namespace TnbIcoms.Application.RoleTransferRequests.Dtos;

public class RoleTransferRequestDto
{
    public int Id { get; set; }
    public string TnbId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? CurrentRoleName { get; set; }
    public string? RequestedRoleName { get; set; }
    public string? CurrentZoneName { get; set; }
    public string? RequestedZoneName { get; set; }
    public string? Reason { get; set; }
    public DateTime RequestedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}

public class CreateRoleTransferRequestDto
{
    public int? RequestedRoleId { get; set; }
    public int? RequestedZoneId { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class RejectRoleTransferRequestDto
{
    public string? Reason { get; set; }
}
