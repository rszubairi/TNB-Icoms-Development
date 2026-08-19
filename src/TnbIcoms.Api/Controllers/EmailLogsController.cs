using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.EmailLogs;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/email-logs")]
public class EmailLogsController : ControllerBase
{
    private readonly IEmailLogService _emailLogService;

    public EmailLogsController(IEmailLogService emailLogService)
    {
        _emailLogService = emailLogService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? status,
        [FromQuery] string? templateCode,
        [FromQuery] string? toAddress,
        [FromQuery] DateTime? dateStart,
        [FromQuery] DateTime? dateEnd)
    {
        var result = await _emailLogService.ListAsync(status, templateCode, toAddress, dateStart, dateEnd);
        return Ok(result);
    }
}
