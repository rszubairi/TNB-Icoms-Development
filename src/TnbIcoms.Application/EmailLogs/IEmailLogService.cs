using TnbIcoms.Application.Common;
using TnbIcoms.Application.EmailLogs.Dtos;

namespace TnbIcoms.Application.EmailLogs;

public interface IEmailLogService
{
    Task<ApiResponse<List<EmailLogDto>>> ListAsync(string? status, string? templateCode, string? toAddress, DateTime? dateStart, DateTime? dateEnd);
}
