namespace TnbIcoms.Domain.Entities.Config;

public class AuthorisationPersonnel
{
    public int AuthorisationPersonnelId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? StaffId { get; set; }
    public int ZoneId { get; set; }
    public string? Designation { get; set; }
    public bool IsActive { get; set; } = true;

    public Zone? Zone { get; set; }
}
