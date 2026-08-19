using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace TnbIcoms.Application.Email;

public class ResendEmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ResendEmailSender(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var apiKey = _configuration["Email:ResendApiKey"];
        var fromAddress = _configuration["Email:FromAddress"] ?? "TNB ICOMS <noreply@tnb-icoms.dev>";

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(new
        {
            from = fromAddress,
            to = new[] { to },
            subject,
            html = htmlBody
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
