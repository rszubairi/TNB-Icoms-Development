namespace TnbIcoms.Application.EmailTemplates.Dtos;

public class EmailTemplateDto
{
    public int EmailTemplateId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string AvailableTags { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UpdatedByName { get; set; }
}

public class UpdateEmailTemplateRequestDto
{
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
