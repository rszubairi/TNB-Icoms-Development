namespace TnbIcoms.Domain.Entities.Audit;

public class AuditLog
{
    public long AuditLogId { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, etc.
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
