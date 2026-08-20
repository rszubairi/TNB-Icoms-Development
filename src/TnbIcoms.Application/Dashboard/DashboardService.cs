using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Dashboard.Dtos;
using TnbIcoms.Application.Outages;
using TnbIcoms.Application.Outages.Dtos;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Dashboard;

public class DashboardService : IDashboardService
{
    private const int RollingWeeks = 12;

    private readonly AppDbContext _dbContext;

    public DashboardService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<DashboardDto>> GetAsync(int? zoneId)
    {
        var query = _dbContext.Outages
            .Include(o => o.Zone)
            .Include(o => o.Station)
            .Include(o => o.VoltageLevel)
            .Include(o => o.PrimaryEquipment)
            .Include(o => o.Project)
            .Include(o => o.Pics)
            .Include(o => o.CreatedByUser)
            .Where(o => !o.IsDeleted)
            .AsQueryable();

        if (zoneId.HasValue) query = query.Where(o => o.ZoneId == zoneId.Value);

        var outages = await query.ToListAsync();
        var dropdownLabels = await _dbContext.DropdownValues.ToDictionaryAsync(d => d.DropdownValueId, d => d.ValueLabel);

        var now = DateTime.UtcNow;
        var windowStart = now.AddDays(-7 * (RollingWeeks - 1));

        // --- Weekly counts (rolling window, by request/created date) ---
        var weekBuckets = new List<WeeklyOutageCountDto>();
        for (var i = RollingWeeks - 1; i >= 0; i--)
        {
            var weekAnchor = now.AddDays(-7 * i);
            var year = ISOWeek.GetYear(weekAnchor);
            var week = ISOWeek.GetWeekOfYear(weekAnchor);
            var weekStart = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
            var weekEnd = weekStart.AddDays(7);

            var count = outages.Count(o => o.CreatedAt >= weekStart && o.CreatedAt < weekEnd);

            weekBuckets.Add(new WeeklyOutageCountDto
            {
                Year = year,
                WeekNumber = week,
                WeekLabel = $"Wk {week}",
                WeekStart = weekStart,
                Count = count
            });
        }

        // --- Status breakdown (RequestorStatus — the primary outage-request lifecycle status) ---
        var statusBreakdown = outages
            .GroupBy(o => o.RequestorStatus)
            .Select(g => new StatusBreakdownDto { Status = g.Key, Count = g.Count() })
            .OrderByDescending(s => s.Count)
            .ToList();

        // --- In Progress summary (max 5, most recently active, not yet closed) ---
        var inProgress = outages
            .Where(o => !o.RequestorStatus.StartsWith("Outage Closed") && !(o.GncStatus ?? string.Empty).StartsWith("Outage Closed"))
            .OrderByDescending(o => o.UpdatedAt ?? o.CreatedAt)
            .Take(5)
            .Select(o => OutageService.MapListItem(o, dropdownLabels))
            .ToList();

        // --- Emergency requests still open ---
        var emergencyRequests = outages
            .Where(o => o.OutageTypeCode == "Emergency" && !o.RequestorStatus.StartsWith("Outage Closed") && !(o.GncStatus ?? string.Empty).StartsWith("Outage Closed"))
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => OutageService.MapListItem(o, dropdownLabels))
            .ToList();

        // --- Headline metrics ---
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var metrics = new DashboardMetricsDto
        {
            TotalOutages = outages.Count,
            PendingPlannerReview = outages.Count(o => o.RequestorStatus == "Pending" && o.PlannerStatus is null),
            PendingGnmApproval = outages.Count(o => o.RequestorStatus == "Confirmed" && o.PlannerStatus == "Agreed" && (o.GnmStatus == "Pending" || o.GnmStatus == "Under-Study")),
            ActiveNow = outages.Count(o => o.GncStatus == "Taken-Active"),
            EmergencyOpen = emergencyRequests.Count,
            ClosedThisMonth = outages.Count(o => o.GncStatus == "Outage Closed" && (o.UpdatedAt ?? o.CreatedAt) >= monthStart)
        };

        return ApiResponse<DashboardDto>.Ok(new DashboardDto
        {
            Metrics = metrics,
            WeeklyOutageCounts = weekBuckets,
            StatusBreakdown = statusBreakdown,
            InProgress = inProgress,
            EmergencyRequests = emergencyRequests
        });
    }
}
