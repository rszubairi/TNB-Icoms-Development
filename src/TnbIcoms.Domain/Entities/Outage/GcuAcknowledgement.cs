namespace TnbIcoms.Domain.Entities.Outage;

public class GcuAcknowledgement
{
    public int GcuAcknowledgementId { get; set; }
    public int OutageId { get; set; }
    public int GcuUserId { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public bool IsAutoAgreed { get; set; } // 7-day DSO auto-agree
    public string? Remark { get; set; }

    public Outage? Outage { get; set; }
}
