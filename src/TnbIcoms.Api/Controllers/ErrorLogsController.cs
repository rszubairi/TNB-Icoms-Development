using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.ErrorLogs;
using TnbIcoms.Application.ErrorLogs.Dtos;

namespace TnbIcoms.Api.Controllers;

[ApiController]
[Route("api/errors")]
public class ErrorLogsController : ControllerBase
{
    private readonly IErrorLogService _errorLogService;

    public ErrorLogsController(IErrorLogService errorLogService)
    {
        _errorLogService = errorLogService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? source, [FromQuery] string? severity, [FromQuery] DateTime? dateStart, [FromQuery] DateTime? dateEnd)
    {
        var result = await _errorLogService.ListAsync(source, severity, dateStart, dateEnd);
        return Ok(result);
    }

    /// <summary>
    /// Frontend ErrorHandler reports here. Allows anonymous because unhandled errors can
    /// happen on the login page, before a token exists.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("client")]
    public async Task<IActionResult> ReportClientError([FromBody] ReportClientErrorRequestDto request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userAgent = Request.Headers.UserAgent.ToString();
            await _errorLogService.LogAsync("Frontend", request.Severity, request.Message, request.StackTrace, request.Url, userId, userAgent);
        }
        catch
        {
            // The client already fire-and-forgets this call; a logging failure here should
            // never itself surface as an error back to the reporting page.
        }

        return Ok(new { success = true });
    }

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : null;
    }
}
