namespace TnbIcoms.Domain.Entities.Config;

public class Station
{
    public int StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public string StationAbbr { get; set; } = string.Empty;
    public int ZoneId { get; set; }
    public int OrgId { get; set; }
    public string? SldFileUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public Zone? Zone { get; set; }
    public Organisation? Organisation { get; set; }
}
