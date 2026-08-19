namespace TnbIcoms.Domain.Entities.Config;

public class Organisation
{
    public int OrganisationId { get; set; }
    public string OrganisationName { get; set; } = string.Empty;
    public string? OrganisationCode { get; set; }
    public bool IsGcu { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
