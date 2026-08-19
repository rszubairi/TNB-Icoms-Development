using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.EquipmentTypes;
using TnbIcoms.Application.EquipmentTypes.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/equipment-types")]
public class EquipmentTypesController : ControllerBase
{
    private readonly IEquipmentTypeService _equipmentTypeService;

    public EquipmentTypesController(IEquipmentTypeService equipmentTypeService)
    {
        _equipmentTypeService = equipmentTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? voltageLevelId)
    {
        var result = await _equipmentTypeService.ListAsync(voltageLevelId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEquipmentTypeRequestDto request)
    {
        var result = await _equipmentTypeService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipmentTypeRequestDto request)
    {
        var result = await _equipmentTypeService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _equipmentTypeService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
