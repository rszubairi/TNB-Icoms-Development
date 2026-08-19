namespace TnbIcoms.Application.Statistics.Dtos;

public class ApprovedOutagesByDepartmentDto
{
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DepartmentStatusSummaryDto
{
    public string Department { get; set; } = string.Empty;
    public int TotalOutages { get; set; }
    public int TakenCompleted { get; set; }
    public int NotTaken { get; set; }
    public int TakenActive { get; set; }
}

public class OutageTypeBreakdownDto
{
    public string OutageTypeCode { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RoutineMaintenanceSummaryDto
{
    public string Department { get; set; } = string.Empty;
    public int Completed { get; set; }
    public int Pending { get; set; }
}

public class StatisticsDashboardDto
{
    public List<ApprovedOutagesByDepartmentDto> ApprovedOutagesByDepartment { get; set; } = new();
    public List<DepartmentStatusSummaryDto> StatusSummaryByDepartment { get; set; } = new();
    public List<OutageTypeBreakdownDto> OutageTypeBreakdown { get; set; } = new();
    public List<RoutineMaintenanceSummaryDto> RoutineMaintenanceByDepartment { get; set; } = new();
}
