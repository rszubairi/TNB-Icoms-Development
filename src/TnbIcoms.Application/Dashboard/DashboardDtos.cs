using TnbIcoms.Application.Outages.Dtos;

namespace TnbIcoms.Application.Dashboard.Dtos;

public class WeeklyOutageCountDto
{
    public int Year { get; set; }
    public int WeekNumber { get; set; }
    public string WeekLabel { get; set; } = string.Empty; // e.g. "Wk 33"
    public DateTime WeekStart { get; set; }
    public int Count { get; set; }
}

public class StatusBreakdownDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardMetricsDto
{
    public int TotalOutages { get; set; }
    public int PendingPlannerReview { get; set; }
    public int PendingGnmApproval { get; set; }
    public int ActiveNow { get; set; }
    public int EmergencyOpen { get; set; }
    public int ClosedThisMonth { get; set; }
}

public class DashboardDto
{
    public DashboardMetricsDto Metrics { get; set; } = new();
    public List<WeeklyOutageCountDto> WeeklyOutageCounts { get; set; } = new();
    public List<StatusBreakdownDto> StatusBreakdown { get; set; } = new();
    public List<OutageListItemDto> InProgress { get; set; } = new();
    public List<OutageListItemDto> EmergencyRequests { get; set; } = new();
}
