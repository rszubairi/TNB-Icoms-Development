using TnbIcoms.Application.Common;
using TnbIcoms.Application.OutageScheduleWindows.Dtos;

namespace TnbIcoms.Application.OutageScheduleWindows;

public interface IOutageScheduleWindowService
{
    Task<ApiResponse<List<OutageScheduleWindowDto>>> ListAsync();
    Task<ApiResponse<List<OutageScheduleWindowDto>>> SaveAsync(SaveScheduleWindowsRequestDto request);
}
