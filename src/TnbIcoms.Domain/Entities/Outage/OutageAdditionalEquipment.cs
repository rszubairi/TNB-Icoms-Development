using TnbIcoms.Domain.Entities.Config;

namespace TnbIcoms.Domain.Entities.Outage;

public class OutageAdditionalEquipment
{
    public int OutageAdditionalEquipmentId { get; set; }
    public int OutageId { get; set; }
    public int EquipmentId { get; set; }

    public Outage? Outage { get; set; }
    public Equipment? Equipment { get; set; }
}
