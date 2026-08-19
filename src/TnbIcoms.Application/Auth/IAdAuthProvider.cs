namespace TnbIcoms.Application.Auth;

public enum AdAuthStatus
{
    Success,
    InvalidCredentials,
    NotSupported
}

public class AdAuthResult
{
    public AdAuthStatus Status { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
}

public interface IAdAuthProvider
{
    Task<AdAuthResult> AuthenticateAsync(string tnbId, string password);
}
