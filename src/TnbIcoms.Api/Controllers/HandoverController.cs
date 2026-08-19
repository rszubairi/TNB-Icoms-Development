using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Handover;
using TnbIcoms.Application.Handover.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/handover")]
public class HandoverController : ControllerBase
{
    private readonly IHandoverService _handoverService;

    public HandoverController(IHandoverService handoverService)
    {
        _handoverService = handoverService;
    }

    [HttpGet("categories")]
    public IActionResult ListCategories() => Ok(new { categories = HandoverCategories.All });

    [HttpGet("shift")]
    public async Task<IActionResult> GetOrCreateShift([FromQuery] DateTime shiftDate, [FromQuery] string shiftType, [FromQuery] int zoneId)
    {
        var result = await _handoverService.GetOrCreateShiftAsync(shiftDate, shiftType, zoneId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("shifts")]
    public async Task<IActionResult> ListShifts([FromQuery] int zoneId, [FromQuery] DateTime? dateStart, [FromQuery] DateTime? dateEnd)
    {
        var result = await _handoverService.ListShiftsAsync(zoneId, dateStart, dateEnd);
        return Ok(result);
    }

    [HttpPut("shifts/{id:int}/control")]
    public async Task<IActionResult> UpdateShiftControl(int id, [FromBody] UpdateShiftControlRequestDto request)
    {
        var result = await _handoverService.UpdateShiftControlAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("shifts/{id:int}/entries")]
    public async Task<IActionResult> AddEntry(int id, [FromBody] AddEntryRequestDto request)
    {
        var result = await _handoverService.AddEntryAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("entries/{id:int}")]
    public async Task<IActionResult> DeleteEntry(int id)
    {
        var result = await _handoverService.DeleteEntryAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("shifts/{id:int}/pass")]
    public async Task<IActionResult> PassHandover(int id)
    {
        var result = await _handoverService.PassHandoverAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
