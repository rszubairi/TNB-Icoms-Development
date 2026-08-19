namespace TnbIcoms.Application.Organisations.Dtos;

public class OrganisationListItemDto
{
    public int OrganisationId { get; set; }
    public string OrganisationName { get; set; } = string.Empty;
    public string OrganisationCode { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public bool IsGcu { get; set; }
    public bool IsActive { get; set; }
}

public class CreateOrganisationRequestDto
{
    public string OrganisationName { get; set; } = string.Empty;
    public string OrganisationCode { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public bool IsGcu { get; set; }
}

public class UpdateOrganisationRequestDto
{
    public string OrganisationName { get; set; } = string.Empty;
    public string OrganisationCode { get; set; } = string.Empty;
    public bool IsGcu { get; set; }
    public bool IsActive { get; set; } = true;
}
