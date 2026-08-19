namespace TnbIcoms.Domain.Entities.Outage;

public class OutagePic
{
    public int OutagePicId { get; set; }
    public int OutageId { get; set; }
    public string PicName { get; set; } = string.Empty;
    public string? PicContact { get; set; }
    public string? PicRole { get; set; } // Requestor PIC, GCU PIC, etc.

    public Outage? Outage { get; set; }
}
