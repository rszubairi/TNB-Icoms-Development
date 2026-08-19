using TnbIcoms.Application.Common;
using TnbIcoms.Application.EmailTemplates.Dtos;

namespace TnbIcoms.Application.EmailTemplates;

public interface IEmailTemplateService
{
    Task<ApiResponse<List<EmailTemplateDto>>> ListAsync();
    Task<ApiResponse<EmailTemplateDto>> GetByCodeAsync(string templateCode);
    Task<ApiResponse<EmailTemplateDto>> UpdateAsync(string templateCode, UpdateEmailTemplateRequestDto request, int currentUserId);

    /// <summary>
    /// Renders the named template's Subject/BodyHtml with {{Tag}} placeholders replaced from
    /// <paramref name="tags"/>. Returns null if the template is missing or inactive — callers
    /// should treat that as "don't send" rather than falling back to a hardcoded string, so an
    /// admin who deactivates a template can suppress that notification entirely.
    /// </summary>
    Task<(string Subject, string Body)?> RenderAsync(string templateCode, IDictionary<string, string> tags);
}
