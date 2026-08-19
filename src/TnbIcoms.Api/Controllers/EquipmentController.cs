using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Equipment;
using TnbIcoms.Application.Equipment.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/equipment")]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int? zoneId,
        [FromQuery] int? stationId,
        [FromQuery] int? voltageLevelId,
        [FromQuery] int? equipmentTypeId)
    {
        var result = await _equipmentService.ListAsync(zoneId, stationId, voltageLevelId, equipmentTypeId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentRequestDto request)
    {
        var result = await _equipmentService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentRequestDto request)
    {
        var result = await _equipmentService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _equipmentService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
