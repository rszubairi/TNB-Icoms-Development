using TnbIcoms.Application.Common;
using TnbIcoms.Application.Statistics.Dtos;

namespace TnbIcoms.Application.Statistics;

public interface IStatisticsService
{
    Task<ApiResponse<StatisticsDashboardDto>> GetDashboardAsync(int year, int? month);
}
