namespace TnbIcoms.Domain.Entities.Config;

public class Zone
{
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public string ZoneAbbr { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ZoneLocation> Locations { get; set; } = new List<ZoneLocation>();
    public ICollection<Station> Stations { get; set; } = new List<Station>();
}
