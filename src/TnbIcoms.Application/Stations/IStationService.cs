using TnbIcoms.Application.Common;
using TnbIcoms.Application.Stations.Dtos;

namespace TnbIcoms.Application.Stations;

public interface IStationService
{
    Task<ApiResponse<List<StationListItemDto>>> ListAsync(int? zoneId, int? orgId);
    Task<ApiResponse<StationListItemDto>> CreateAsync(CreateStationRequestDto request);
    Task<ApiResponse<StationListItemDto>> UpdateAsync(int stationId, UpdateStationRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int stationId);
}
