namespace TnbIcoms.Application.ErrorLogs.Dtos;

public class ErrorLogDto
{
    public long ErrorLogId { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Url { get; set; }
    public string? UserName { get; set; }
    public string? UserAgent { get; set; }
    public DateTime OccurredAt { get; set; }
}

public class ReportClientErrorRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Url { get; set; }
    public string Severity { get; set; } = "Error";
}
