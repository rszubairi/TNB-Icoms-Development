using TnbIcoms.Domain.Entities.Audit;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Email;

/// <summary>
/// Decorator that records every email the system attempts to send — success or failure —
/// to the EmailLog table, then delegates to the real transport. Registered in place of the
/// underlying sender so no call site has to remember to log anything itself. A send failure
/// is logged and swallowed rather than propagated, so a transient email outage never breaks
/// the business action (outage creation, user onboarding, etc.) that triggered it.
/// </summary>
public class LoggingEmailSender : IEmailSender
{
    private readonly IEmailSender _inner;
    private readonly AppDbContext _dbContext;

    public LoggingEmailSender(IEmailSender inner, AppDbContext dbContext)
    {
        _inner = inner;
        _dbContext = dbContext;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, string? templateCode = null)
    {
        var log = new EmailLog
        {
            TemplateCode = templateCode,
            ToAddress = to,
            Subject = subject,
            BodyHtml = htmlBody,
            SentAt = DateTime.UtcNow
        };

        try
        {
            await _inner.SendAsync(to, subject, htmlBody, templateCode);
            log.Status = "Sent";
        }
        catch (Exception ex)
        {
            log.Status = "Failed";
            log.ErrorMessage = ex.Message;
        }

        _dbContext.EmailLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }
}
