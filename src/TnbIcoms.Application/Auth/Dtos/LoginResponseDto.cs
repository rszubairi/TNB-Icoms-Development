namespace TnbIcoms.Application.Auth.Dtos;

public class LoginResponseDto
{
    public bool RequiresTwoFactor { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserSummaryDto? User { get; set; }
}

public class UserSummaryDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? TnbId { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public int ZoneId { get; set; }
    public string? ZoneName { get; set; }
}
