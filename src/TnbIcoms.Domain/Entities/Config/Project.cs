namespace TnbIcoms.Domain.Entities.Config;

public class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectCode { get; set; }
    public int? ZoneId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Zone? Zone { get; set; }
}
