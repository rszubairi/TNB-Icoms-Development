namespace TnbIcoms.Application.Reports.Dtos;

public class ReportFilterDto
{
    public int? ZoneId { get; set; }
    public int? StationId { get; set; }
    public int? JobTypeId { get; set; }
    public string? OutageCode { get; set; }
    public string? RequestorStatus { get; set; }
    public string? GnmStatus { get; set; }
    public string? Keyword { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public bool ShowDraft { get; set; }
    public string? SortBy { get; set; } // "date" | "code"
}

public class SavedReportFilterDto
{
    public int SavedReportFilterId { get; set; }
    public string FilterName { get; set; } = string.Empty;
    public ReportFilterDto Filter { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class SaveReportFilterRequestDto
{
    public string FilterName { get; set; } = string.Empty;
    public ReportFilterDto Filter { get; set; } = new();
}
