using TnbIcoms.Domain.Entities.Auth;
namespace TnbIcoms.Application.Auth.Dtos;
public class GatewayLookupResponse
{
    public GatewayIdentity? Identity { get; set; }
    public User? DomainUser { get; set; }
}
public class GatewayIdentity
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? TnbId { get; set; }
    public bool IsActive { get; set; }
}
