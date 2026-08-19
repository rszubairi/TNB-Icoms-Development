namespace TnbIcoms.Domain.Entities.Outage;

public class Authorisation
{
    public int AuthorisationId { get; set; }
    public int OutageId { get; set; }
    public string AuthorisationNo { get; set; } = string.Empty;
    public int PersonnelId { get; set; }
    public DateTime TakenActiveAt { get; set; }
    public DateTime? TakenCompletedAt { get; set; }
    public string? Remark { get; set; }
    public DateTime? ExtendedTo { get; set; } // set by GNC when an active outage overruns its planned end and is kept open deliberately
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Outage? Outage { get; set; }
}
