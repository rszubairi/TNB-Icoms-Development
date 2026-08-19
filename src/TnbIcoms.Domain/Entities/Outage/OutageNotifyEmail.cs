namespace TnbIcoms.Domain.Entities.Outage;

public class OutageNotifyEmail
{
    public int OutageNotifyEmailId { get; set; }
    public int OutageId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsNotified { get; set; }
    public DateTime? NotifiedAt { get; set; }

    public Outage? Outage { get; set; }
}
