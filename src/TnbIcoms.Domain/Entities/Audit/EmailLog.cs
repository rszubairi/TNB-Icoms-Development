namespace TnbIcoms.Domain.Entities.Audit;

public class EmailLog
{
    public long EmailLogId { get; set; }
    public string? TemplateCode { get; set; }
    public string ToAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string Status { get; set; } = "Sent"; // Sent, Failed
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
