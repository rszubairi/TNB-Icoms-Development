namespace TnbIcoms.Domain.Entities.Outage;

public class SingleLineDiagram
{
    public int SingleLineDiagramId { get; set; }
    public int OutageId { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public int RevisionNo { get; set; } = 1;
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Outage? Outage { get; set; }
}
