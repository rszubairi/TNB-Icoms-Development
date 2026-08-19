using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Gnc;
using TnbIcoms.Application.Gnc.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/gnc")]
public class GncController : ControllerBase
{
    private readonly IGncService _gncService;

    public GncController(IGncService gncService)
    {
        _gncService = gncService;
    }

    [HttpGet("scheduled")]
    public async Task<IActionResult> ListScheduled([FromQuery] int? zoneId)
    {
        var result = await _gncService.ListScheduledAsync(zoneId);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> ListActive([FromQuery] int? zoneId)
    {
        var result = await _gncService.ListActiveAsync(zoneId);
        return Ok(result);
    }

    [HttpGet("authorisation-in-force")]
    public async Task<IActionResult> ListAuthorisationInForce([FromQuery] int? zoneId)
    {
        var result = await _gncService.ListAuthorisationInForceAsync(zoneId);
        return Ok(result);
    }

    [HttpPost("outages/{id:int}/take-active")]
    public async Task<IActionResult> TakeActive(int id, [FromBody] TakeActiveRequestDto request)
    {
        var result = await _gncService.TakeActiveAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("outages/{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteAuthorisationRequestDto request)
    {
        var result = await _gncService.CompleteAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("outages/{id:int}/extend")]
    public async Task<IActionResult> Extend(int id, [FromBody] ExtendAuthorisationRequestDto request)
    {
        var result = await _gncService.ExtendAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("outages/{id:int}/close")]
    public async Task<IActionResult> Close(int id)
    {
        var result = await _gncService.CloseAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("outages/{id:int}/not-taken")]
    public async Task<IActionResult> NotTaken(int id, [FromBody] NotTakenRequestDto request)
    {
        var result = await _gncService.NotTakenAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("outages/{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelOutageRequestDto request)
    {
        var result = await _gncService.CancelAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("forced-outages")]
    public async Task<IActionResult> CreateForcedOutage([FromBody] ForcedOutageRequestDto request)
    {
        var result = await _gncService.CreateForcedOutageAsync(request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
