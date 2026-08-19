using TnbIcoms.Application.Common;
using TnbIcoms.Application.EquipmentTypes.Dtos;

namespace TnbIcoms.Application.EquipmentTypes;

public interface IEquipmentTypeService
{
    Task<ApiResponse<List<EquipmentTypeDto>>> ListAsync(int? voltageLevelId);
    Task<ApiResponse<EquipmentTypeDto>> CreateAsync(CreateEquipmentTypeRequestDto request);
    Task<ApiResponse<EquipmentTypeDto>> UpdateAsync(int equipmentTypeId, UpdateEquipmentTypeRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int equipmentTypeId);
}
