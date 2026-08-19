namespace TnbIcoms.Application.Auth.Dtos;

public class LoginRequestDto
{
    public string StaffIdOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberDevice { get; set; }
}
