using TnbIcoms.Application.Common;

namespace TnbIcoms.Application.Lookups;

public class ZoneLookupDto
{
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public string ZoneAbbr { get; set; } = string.Empty;
}

public interface IZoneService
{
    Task<ApiResponse<List<ZoneLookupDto>>> ListAsync();
}
