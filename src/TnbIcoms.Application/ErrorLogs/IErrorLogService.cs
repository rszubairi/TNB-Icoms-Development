using TnbIcoms.Application.Common;
using TnbIcoms.Application.ErrorLogs.Dtos;

namespace TnbIcoms.Application.ErrorLogs;

public interface IErrorLogService
{
    Task LogAsync(string source, string severity, string message, string? stackTrace, string? url, int? userId, string? userAgent);
    Task<ApiResponse<List<ErrorLogDto>>> ListAsync(string? source, string? severity, DateTime? dateStart, DateTime? dateEnd);
}
