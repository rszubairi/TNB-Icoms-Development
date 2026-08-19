namespace TnbIcoms.Application.Email;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string htmlBody, string? templateCode = null);
}
