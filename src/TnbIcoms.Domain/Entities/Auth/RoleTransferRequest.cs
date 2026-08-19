namespace TnbIcoms.Domain.Entities.Auth;

public class RoleTransferRequest
{
    public int RoleTransferRequestId { get; set; }
    public int UserId { get; set; }
    public int FromRoleId { get; set; }
    public int ToRoleId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? Reason { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public int RequestedBy { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RejectionReason { get; set; }

    public User? User { get; set; }
    public Role? FromRole { get; set; }
    public Role? ToRole { get; set; }
}
