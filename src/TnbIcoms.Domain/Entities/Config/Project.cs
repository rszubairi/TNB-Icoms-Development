namespace TnbIcoms.Domain.Entities.Config;

public class Project
{
    public int ProjectId { get; set; }
    public string TpCode { get; set; } = string.Empty; // Unique, non-reusable
    public string ProjectSuffix { get; set; } = string.Empty; // Reusable across projects
    public string ProjectName { get; set; } = string.Empty; // Computed: TpCode + " - " + ProjectSuffix
    public int? ZoneId { get; set; }
    public DateTime? StartDate { get; set; } // Auto-filled from first linked outage
    public DateTime? EndDate { get; set; } // Auto-filled when TOMS closes the project
    public bool IsActive { get; set; } = true; // Open/Closed

    public Zone? Zone { get; set; }
}
