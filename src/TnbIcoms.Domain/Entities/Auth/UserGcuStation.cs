namespace TnbIcoms.Domain.Entities.Auth;

public class UserGcuStation
{
    public int UserGcuStationId { get; set; }
    public int UserId { get; set; }
    public int StationId { get; set; }

    public User? User { get; set; }
    public Config.Station? Station { get; set; }
}
