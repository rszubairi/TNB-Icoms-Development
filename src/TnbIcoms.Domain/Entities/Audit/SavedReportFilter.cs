namespace TnbIcoms.Domain.Entities.Audit;

public class SavedReportFilter
{
    public int SavedReportFilterId { get; set; }
    public int UserId { get; set; }
    public string FilterName { get; set; } = string.Empty;
    public string ReportCode { get; set; } = string.Empty;
    public string FilterJson { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
