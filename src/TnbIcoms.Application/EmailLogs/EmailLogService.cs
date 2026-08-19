using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.EmailLogs.Dtos;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.EmailLogs;

public class EmailLogService : IEmailLogService
{
    private readonly AppDbContext _dbContext;

    public EmailLogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<EmailLogDto>>> ListAsync(string? status, string? templateCode, string? toAddress, DateTime? dateStart, DateTime? dateEnd)
    {
        var query = _dbContext.EmailLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(e => e.Status == status);
        if (!string.IsNullOrWhiteSpace(templateCode)) query = query.Where(e => e.TemplateCode == templateCode);
        if (!string.IsNullOrWhiteSpace(toAddress)) query = query.Where(e => e.ToAddress.Contains(toAddress));
        if (dateStart.HasValue) query = query.Where(e => e.SentAt >= dateStart.Value);
        if (dateEnd.HasValue) query = query.Where(e => e.SentAt <= dateEnd.Value);

        var logs = await query.OrderByDescending(e => e.SentAt).Take(500).ToListAsync();

        return ApiResponse<List<EmailLogDto>>.Ok(logs.Select(e => new EmailLogDto
        {
            EmailLogId = e.EmailLogId,
            TemplateCode = e.TemplateCode,
            ToAddress = e.ToAddress,
            Subject = e.Subject,
            BodyHtml = e.BodyHtml,
            Status = e.Status,
            ErrorMessage = e.ErrorMessage,
            SentAt = e.SentAt
        }).ToList());
    }
}
