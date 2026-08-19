namespace TnbIcoms.Application.EmailLogs.Dtos;

public class EmailLogDto
{
    public long EmailLogId { get; set; }
    public string? TemplateCode { get; set; }
    public string ToAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; }
}
