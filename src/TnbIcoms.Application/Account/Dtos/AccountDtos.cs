namespace TnbIcoms.Application.Account.Dtos;

public class AccountProfileDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? RoleName { get; set; }
    public string? ZoneName { get; set; }
    public bool IsExternal { get; set; } // true = Membership auth (can self-service password); false = AD-managed
}

public class UpdateAccountProfileRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}

public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}
