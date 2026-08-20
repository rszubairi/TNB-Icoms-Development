using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TnbIcoms.Application.Auth;

public class LdapAdAuthProvider : IAdAuthProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LdapAdAuthProvider> _logger;

    public LdapAdAuthProvider(IConfiguration configuration, ILogger<LdapAdAuthProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<AdAuthResult> AuthenticateAsync(string tnbId, string password)
    {
        var domain = _configuration["Ad:Domain"];
        if (string.IsNullOrWhiteSpace(domain))
        {
            _logger.LogWarning("AD authentication requested but Ad:Domain is not configured.");
            return Task.FromResult(new AdAuthResult { Status = AdAuthStatus.NotSupported });
        }

        try
        {
            using var context = new PrincipalContext(ContextType.Domain, domain);
            var isValid = context.ValidateCredentials(tnbId, password);
            if (!isValid)
            {
                return Task.FromResult(new AdAuthResult { Status = AdAuthStatus.InvalidCredentials });
            }

            using var userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, tnbId);
            return Task.FromResult(new AdAuthResult
            {
                Status = AdAuthStatus.Success,
                DisplayName = userPrincipal?.DisplayName,
                Email = userPrincipal?.EmailAddress
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AD authentication failed for {TnbId} against domain {Domain}.", tnbId, domain);
            return Task.FromResult(new AdAuthResult { Status = AdAuthStatus.NotSupported });
        }
    }
}
