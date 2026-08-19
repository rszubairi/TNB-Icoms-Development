namespace TnbIcoms.Application.Users.Dtos;

public class CreateUserRequestDto
{
    public string TnbId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public int RoleId { get; set; }
    public int ZoneId { get; set; }
    public int? OrganisationId { get; set; }
    public int? GcuTypeId { get; set; }
    public List<int> GcuStationIds { get; set; } = new();
}
