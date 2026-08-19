using TnbIcoms.Application.AuthorisationPersonnel.Dtos;
using TnbIcoms.Application.Common;

namespace TnbIcoms.Application.AuthorisationPersonnel;

public interface IAuthorisationPersonnelService
{
    Task<ApiResponse<List<AuthorisationPersonnelDto>>> ListAsync(int? zoneId);
    Task<ApiResponse<AuthorisationPersonnelDto>> CreateAsync(SaveAuthorisationPersonnelRequestDto request);
    Task<ApiResponse<AuthorisationPersonnelDto>> UpdateAsync(int personnelId, SaveAuthorisationPersonnelRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int personnelId);
}
