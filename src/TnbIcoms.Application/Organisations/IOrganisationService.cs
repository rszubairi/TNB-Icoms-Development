using TnbIcoms.Application.Common;
using TnbIcoms.Application.Organisations.Dtos;

namespace TnbIcoms.Application.Organisations;

public interface IOrganisationService
{
    Task<ApiResponse<List<OrganisationListItemDto>>> ListAsync(int? zoneId);
    Task<ApiResponse<OrganisationListItemDto>> CreateAsync(CreateOrganisationRequestDto request);
    Task<ApiResponse<OrganisationListItemDto>> UpdateAsync(int organisationId, UpdateOrganisationRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int organisationId);
}
