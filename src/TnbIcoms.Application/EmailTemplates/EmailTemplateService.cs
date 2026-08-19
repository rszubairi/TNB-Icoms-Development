using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.EmailTemplates.Dtos;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.EmailTemplates;

public class EmailTemplateService : IEmailTemplateService
{
    private readonly AppDbContext _dbContext;

    public EmailTemplateService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<EmailTemplateDto>>> ListAsync()
    {
        var templates = await _dbContext.EmailTemplates.OrderBy(t => t.Name).ToListAsync();
        var userNames = await GetUserNamesAsync();
        return ApiResponse<List<EmailTemplateDto>>.Ok(templates.Select(t => Map(t, userNames)).ToList());
    }

    public async Task<ApiResponse<EmailTemplateDto>> GetByCodeAsync(string templateCode)
    {
        var template = await _dbContext.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateCode == templateCode);
        if (template is null) return ApiResponse<EmailTemplateDto>.Fail("Template not found.");

        var userNames = await GetUserNamesAsync();
        return ApiResponse<EmailTemplateDto>.Ok(Map(template, userNames));
    }

    public async Task<ApiResponse<EmailTemplateDto>> UpdateAsync(string templateCode, UpdateEmailTemplateRequestDto request, int currentUserId)
    {
        var template = await _dbContext.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateCode == templateCode);
        if (template is null) return ApiResponse<EmailTemplateDto>.Fail("Template not found.");
        if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.BodyHtml))
        {
            return ApiResponse<EmailTemplateDto>.Fail("Subject and Body are required.");
        }

        template.Subject = request.Subject;
        template.BodyHtml = request.BodyHtml;
        template.IsActive = request.IsActive;
        template.UpdatedAt = DateTime.UtcNow;
        template.UpdatedBy = currentUserId;
        await _dbContext.SaveChangesAsync();

        var userNames = await GetUserNamesAsync();
        return ApiResponse<EmailTemplateDto>.Ok(Map(template, userNames));
    }

    public async Task<(string Subject, string Body)?> RenderAsync(string templateCode, IDictionary<string, string> tags)
    {
        var template = await _dbContext.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateCode == templateCode);
        if (template is null || !template.IsActive) return null;

        return (Substitute(template.Subject, tags), Substitute(template.BodyHtml, tags));
    }

    private static string Substitute(string text, IDictionary<string, string> tags)
    {
        foreach (var (key, value) in tags)
        {
            text = text.Replace("{{" + key + "}}", value);
        }
        return text;
    }

    private async Task<Dictionary<int, string>> GetUserNamesAsync()
    {
        return await _dbContext.AppUsers.ToDictionaryAsync(u => u.UserId, u => u.FullName);
    }

    private static EmailTemplateDto Map(Domain.Entities.Config.EmailTemplate t, Dictionary<int, string> userNames)
    {
        return new EmailTemplateDto
        {
            EmailTemplateId = t.EmailTemplateId,
            TemplateCode = t.TemplateCode,
            Name = t.Name,
            Subject = t.Subject,
            BodyHtml = t.BodyHtml,
            AvailableTags = t.AvailableTags,
            IsActive = t.IsActive,
            UpdatedAt = t.UpdatedAt,
            UpdatedByName = t.UpdatedBy.HasValue && userNames.TryGetValue(t.UpdatedBy.Value, out var n) ? n : null
        };
    }
}
