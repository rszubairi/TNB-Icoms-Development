namespace TnbIcoms.Application.AuthorisationPersonnel.Dtos;

public class AuthorisationPersonnelDto
{
    public int AuthorisationPersonnelId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? StaffId { get; set; }
    public int ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public string? Designation { get; set; }
    public bool IsActive { get; set; }
}

public class SaveAuthorisationPersonnelRequestDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? StaffId { get; set; }
    public int ZoneId { get; set; }
    public string? Designation { get; set; }
    public bool IsActive { get; set; } = true;
}
