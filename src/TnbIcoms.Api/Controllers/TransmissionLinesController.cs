using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.TransmissionLines;
using TnbIcoms.Application.TransmissionLines.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/transmission-lines")]
public class TransmissionLinesController : ControllerBase
{
    private readonly ITransmissionLineService _lineService;

    public TransmissionLinesController(ITransmissionLineService lineService)
    {
        _lineService = lineService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _lineService.ListAsync();
        return Ok(result);
    }

    [HttpPost("preview")]
    public async Task<IActionResult> Preview([FromBody] TransmissionLineRequestDto request)
    {
        var result = await _lineService.PreviewAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransmissionLineRequestDto request)
    {
        var result = await _lineService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/owner-zones")]
    public async Task<IActionResult> AddOwnerZone(int id, [FromBody] AddOwnerZoneRequestDto request)
    {
        var result = await _lineService.AddOwnerZoneAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}/owner-zones/{zoneId:int}")]
    public async Task<IActionResult> RemoveOwnerZone(int id, int zoneId)
    {
        var result = await _lineService.RemoveOwnerZoneAsync(id, zoneId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _lineService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
