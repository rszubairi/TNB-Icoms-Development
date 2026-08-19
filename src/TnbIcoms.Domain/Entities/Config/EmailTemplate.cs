namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// Self-managed email template (IT Admin editable). TemplateCode is the stable key that
/// application code references when dispatching an email; Subject/BodyHtml may contain
/// {{TagName}} placeholders which are substituted at send time from the tags supplied by
/// the call site. AvailableTags documents which tags that call site provides.
/// </summary>
public class EmailTemplate
{
    public int EmailTemplateId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public string AvailableTags { get; set; } = string.Empty; // comma-separated, informational
    public bool IsActive { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? UpdatedBy { get; set; }
}
