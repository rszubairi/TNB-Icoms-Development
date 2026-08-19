using Microsoft.AspNetCore.Identity;

namespace TnbIcoms.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? TnbId { get; set; }
    public bool IsActive { get; set; } = true;
}
