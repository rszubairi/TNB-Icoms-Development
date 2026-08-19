using TnbIcoms.Application.Common;
using TnbIcoms.Application.Equipment.Dtos;

namespace TnbIcoms.Application.Equipment;

public interface IEquipmentService
{
    Task<ApiResponse<List<EquipmentListItemDto>>> ListAsync(int? zoneId, int? stationId, int? voltageLevelId, int? equipmentTypeId);
    Task<ApiResponse<EquipmentListItemDto>> CreateAsync(CreateEquipmentRequestDto request);
    Task<ApiResponse<EquipmentListItemDto>> UpdateAsync(int equipmentId, UpdateEquipmentRequestDto request);
    Task<ApiResponse<object>> DeactivateAsync(int equipmentId);
}
