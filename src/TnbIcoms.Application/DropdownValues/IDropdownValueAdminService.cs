using TnbIcoms.Application.Common;
using TnbIcoms.Application.DropdownValues.Dtos;

namespace TnbIcoms.Application.DropdownValues;

public interface IDropdownValueAdminService
{
    Task<ApiResponse<List<DropdownValueDto>>> ListAsync(string category);
    Task<ApiResponse<DropdownValueDto>> CreateAsync(CreateDropdownValueRequestDto request);
    Task<ApiResponse<DropdownValueDto>> UpdateAsync(int dropdownValueId, UpdateDropdownValueRequestDto request);
    Task<ApiResponse<object>> ReorderAsync(int dropdownValueId, ReorderDropdownValueRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int dropdownValueId);
}
