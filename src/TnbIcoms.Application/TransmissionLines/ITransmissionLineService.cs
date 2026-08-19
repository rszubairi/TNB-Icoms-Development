using TnbIcoms.Application.Common;
using TnbIcoms.Application.TransmissionLines.Dtos;

namespace TnbIcoms.Application.TransmissionLines;

public interface ITransmissionLineService
{
    Task<ApiResponse<List<TransmissionLineDto>>> ListAsync();
    Task<ApiResponse<List<GeneratedNameDto>>> PreviewAsync(TransmissionLineRequestDto request);
    Task<ApiResponse<TransmissionLineDto>> CreateAsync(TransmissionLineRequestDto request);
    Task<ApiResponse<TransmissionLineDto>> AddOwnerZoneAsync(int transmissionLineId, AddOwnerZoneRequestDto request);
    Task<ApiResponse<object>> RemoveOwnerZoneAsync(int transmissionLineId, int zoneId);
    Task<ApiResponse<object>> DeactivateAsync(int transmissionLineId);
}
