namespace TnbIcoms.Application.Users.Dtos;

public class UserDetailDto
{
    public int UserId { get; set; }
    public string? TnbId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public int ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public int? OrganisationId { get; set; }
    public int? GcuTypeId { get; set; }
    public List<int> GcuStationIds { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
