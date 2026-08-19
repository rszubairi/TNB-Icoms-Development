using Microsoft.Extensions.Logging;

namespace TnbIcoms.Application.Email;

public class StubEmailSender : IEmailSender
{
    private readonly ILogger<StubEmailSender> _logger;

    public StubEmailSender(ILogger<StubEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string htmlBody, string? templateCode = null)
    {
        _logger.LogInformation("[StubEmailSender] To: {To} | Subject: {Subject}\n{Body}", to, subject, htmlBody);
        return Task.CompletedTask;
    }
}
