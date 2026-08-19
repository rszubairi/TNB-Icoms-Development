using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.OutageScheduleWindows;
using TnbIcoms.Application.OutageScheduleWindows.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/outage-schedule-windows")]
public class OutageScheduleWindowsController : ControllerBase
{
    private readonly IOutageScheduleWindowService _service;

    public OutageScheduleWindowsController(IOutageScheduleWindowService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _service.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveScheduleWindowsRequestDto request)
    {
        var result = await _service.SaveAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
