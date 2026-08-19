using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Statistics.Dtos;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Statistics;

public class StatisticsService : IStatisticsService
{
    private readonly AppDbContext _dbContext;

    public StatisticsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<StatisticsDashboardDto>> GetDashboardAsync(int year, int? month)
    {
        var baseQuery = _dbContext.Outages
            .Include(o => o.Zone)
            .Where(o => !o.IsDeleted && o.PlannedStartAt.Year == year);

        if (month.HasValue)
        {
            baseQuery = baseQuery.Where(o => o.PlannedStartAt.Month == month.Value);
        }

        var outages = await baseQuery.ToListAsync();

        var approvedByDept = outages
            .Where(o => o.GnmStatus == "Approved")
            .GroupBy(o => o.Zone?.ZoneName ?? "Unassigned")
            .Select(g => new ApprovedOutagesByDepartmentDto { Department = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        var statusSummary = outages
            .GroupBy(o => o.Zone?.ZoneName ?? "Unassigned")
            .Select(g => new DepartmentStatusSummaryDto
            {
                Department = g.Key,
                TotalOutages = g.Count(),
                TakenCompleted = g.Count(o => o.GncStatus == "Taken-Completed"),
                NotTaken = g.Count(o => o.GncStatus == "Outage Closed - Not Taken"),
                TakenActive = g.Count(o => o.GncStatus == "Taken-Active")
            })
            .OrderByDescending(x => x.TotalOutages)
            .ToList();

        var typeBreakdown = outages
            .GroupBy(o => o.OutageTypeCode)
            .Select(g => new OutageTypeBreakdownDto { OutageTypeCode = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        var routineMaintenance = outages
            .Where(o => o.OutageClass == "Maintenance")
            .GroupBy(o => o.Zone?.ZoneName ?? "Unassigned")
            .Select(g => new RoutineMaintenanceSummaryDto
            {
                Department = g.Key,
                Completed = g.Count(o => o.GncStatus == "Taken-Completed" || o.GncStatus == "Outage Closed"),
                Pending = g.Count(o => o.GncStatus != "Taken-Completed" && o.GncStatus != "Outage Closed")
            })
            .OrderByDescending(x => x.Completed + x.Pending)
            .ToList();

        return ApiResponse<StatisticsDashboardDto>.Ok(new StatisticsDashboardDto
        {
            ApprovedOutagesByDepartment = approvedByDept,
            StatusSummaryByDepartment = statusSummary,
            OutageTypeBreakdown = typeBreakdown,
            RoutineMaintenanceByDepartment = routineMaintenance
        });
    }
}
