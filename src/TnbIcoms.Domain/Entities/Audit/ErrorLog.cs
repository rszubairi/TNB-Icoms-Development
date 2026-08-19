namespace TnbIcoms.Domain.Entities.Audit;

public class ErrorLog
{
    public long ErrorLogId { get; set; }
    public string Source { get; set; } = string.Empty; // Backend, Frontend
    public string Severity { get; set; } = "Error"; // Error, Warning
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Url { get; set; }
    public int? UserId { get; set; }
    public string? UserAgent { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
