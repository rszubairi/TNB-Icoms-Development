namespace TnbIcoms.Domain.Entities.Auth;

public class User
{
    public int UserId { get; set; }
    public string? TnbId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public byte AuthType { get; set; } // 1 = AD (Internal), 2 = Membership (External)
    public string? AspNetUserId { get; set; }
    public int RoleId { get; set; }
    public int ZoneId { get; set; }
    public int? OrganisationId { get; set; }
    public int? GcuTypeId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public Role? Role { get; set; }
    public Config.Zone? Zone { get; set; }
    public Config.Organisation? Organisation { get; set; }
    public ICollection<UserGcuStation> GcuStations { get; set; } = new List<UserGcuStation>();
}
