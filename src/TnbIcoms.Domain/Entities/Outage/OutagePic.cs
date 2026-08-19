namespace TnbIcoms.Domain.Entities.Outage;

public class OutagePic
{
    public int OutagePicId { get; set; }
    public int OutageId { get; set; }
    public string PicName { get; set; } = string.Empty;
    public string PicEmail { get; set; } = string.Empty;
    public string? PicPhone { get; set; }

    public Outage? Outage { get; set; }
}
