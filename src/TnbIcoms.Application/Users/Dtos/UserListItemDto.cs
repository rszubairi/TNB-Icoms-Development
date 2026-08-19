namespace TnbIcoms.Application.Users.Dtos;

public class UserListItemDto
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
    public bool IsActive { get; set; }
}
