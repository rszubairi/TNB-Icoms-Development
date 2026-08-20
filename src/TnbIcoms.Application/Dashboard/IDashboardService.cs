using TnbIcoms.Application.Common;
using TnbIcoms.Application.Dashboard.Dtos;

namespace TnbIcoms.Application.Dashboard;

public interface IDashboardService
{
    Task<ApiResponse<DashboardDto>> GetAsync(int? zoneId);
}
