using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.ErrorLogs.Dtos;
using TnbIcoms.Domain.Entities.Audit;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.ErrorLogs;

public class ErrorLogService : IErrorLogService
{
    private readonly AppDbContext _dbContext;

    public ErrorLogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task LogAsync(string source, string severity, string message, string? stackTrace, string? url, int? userId, string? userAgent)
    {
        _dbContext.ErrorLogs.Add(new ErrorLog
        {
            Source = source,
            Severity = severity,
            Message = message.Length > 4000 ? message[..4000] : message,
            StackTrace = stackTrace,
            Url = url,
            UserId = userId,
            UserAgent = userAgent,
            OccurredAt = DateTime.UtcNow
        });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ApiResponse<List<ErrorLogDto>>> ListAsync(string? source, string? severity, DateTime? dateStart, DateTime? dateEnd)
    {
        var query = _dbContext.ErrorLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(source)) query = query.Where(e => e.Source == source);
        if (!string.IsNullOrWhiteSpace(severity)) query = query.Where(e => e.Severity == severity);
        if (dateStart.HasValue) query = query.Where(e => e.OccurredAt >= dateStart.Value);
        if (dateEnd.HasValue) query = query.Where(e => e.OccurredAt <= dateEnd.Value);

        var errors = await query.OrderByDescending(e => e.OccurredAt).Take(500).ToListAsync();
        var userIds = errors.Where(e => e.UserId.HasValue).Select(e => e.UserId!.Value).Distinct().ToList();
        var userNames = userIds.Count > 0
            ? await _dbContext.AppUsers.Where(u => userIds.Contains(u.UserId)).ToDictionaryAsync(u => u.UserId, u => u.FullName)
            : new Dictionary<int, string>();

        return ApiResponse<List<ErrorLogDto>>.Ok(errors.Select(e => new ErrorLogDto
        {
            ErrorLogId = e.ErrorLogId,
            Source = e.Source,
            Severity = e.Severity,
            Message = e.Message,
            StackTrace = e.StackTrace,
            Url = e.Url,
            UserName = e.UserId.HasValue && userNames.TryGetValue(e.UserId.Value, out var n) ? n : null,
            UserAgent = e.UserAgent,
            OccurredAt = e.OccurredAt
        }).ToList());
    }
}
