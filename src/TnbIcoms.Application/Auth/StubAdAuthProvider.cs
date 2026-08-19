namespace TnbIcoms.Application.Auth;

public class StubAdAuthProvider : IAdAuthProvider
{
    public Task<AdAuthResult> AuthenticateAsync(string tnbId, string password)
    {
        return Task.FromResult(new AdAuthResult { Status = AdAuthStatus.NotSupported });
    }
}
