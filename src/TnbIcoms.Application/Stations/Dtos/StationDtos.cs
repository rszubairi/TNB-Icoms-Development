namespace TnbIcoms.Application.Stations.Dtos;

public class StationListItemDto
{
    public int StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public string StationAbbr { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public int OrgId { get; set; }
    public string? OrganisationName { get; set; }
    public string? SldFileUrl { get; set; }
    public bool IsActive { get; set; }
}

public class CreateStationRequestDto
{
    public string StationName { get; set; } = string.Empty;
    public string StationAbbr { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public int OrgId { get; set; }
}

public class UpdateStationRequestDto
{
    public string StationName { get; set; } = string.Empty;
    public string StationAbbr { get; set; } = string.Empty;
    public int OrgId { get; set; }
    public bool IsActive { get; set; } = true;
}
