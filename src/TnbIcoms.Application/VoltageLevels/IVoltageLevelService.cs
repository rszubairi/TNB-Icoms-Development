using TnbIcoms.Application.Common;
using TnbIcoms.Application.VoltageLevels.Dtos;

namespace TnbIcoms.Application.VoltageLevels;

public interface IVoltageLevelService
{
    Task<ApiResponse<List<VoltageLevelDto>>> ListAsync();
    Task<ApiResponse<VoltageLevelDto>> CreateAsync(CreateVoltageLevelRequestDto request);
    Task<ApiResponse<VoltageLevelDto>> UpdateAsync(int voltageLevelId, UpdateVoltageLevelRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int voltageLevelId);
}
