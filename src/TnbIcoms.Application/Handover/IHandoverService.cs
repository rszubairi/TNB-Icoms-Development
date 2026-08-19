using TnbIcoms.Application.Common;
using TnbIcoms.Application.Handover.Dtos;

namespace TnbIcoms.Application.Handover;

public interface IHandoverService
{
    Task<ApiResponse<HandoverShiftDto>> GetOrCreateShiftAsync(DateTime shiftDate, string shiftType, int zoneId);
    Task<ApiResponse<List<HandoverShiftSummaryDto>>> ListShiftsAsync(int zoneId, DateTime? dateStart, DateTime? dateEnd);
    Task<ApiResponse<object>> UpdateShiftControlAsync(int shiftId, UpdateShiftControlRequestDto request, int currentUserId);
    Task<ApiResponse<HandoverEntryDto>> AddEntryAsync(int shiftId, AddEntryRequestDto request, int currentUserId);
    Task<ApiResponse<object>> DeleteEntryAsync(int entryId, int currentUserId);
    Task<ApiResponse<HandoverShiftDto>> PassHandoverAsync(int shiftId, int currentUserId);
}
