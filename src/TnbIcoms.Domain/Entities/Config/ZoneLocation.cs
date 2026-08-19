namespace TnbIcoms.Domain.Entities.Config;

public class ZoneLocation
{
    public int ZoneLocationId { get; set; }
    public int ZoneId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsActive { get; set; } = true;

    public Zone? Zone { get; set; }
}
