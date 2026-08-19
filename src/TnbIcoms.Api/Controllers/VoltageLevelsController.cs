using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.VoltageLevels;
using TnbIcoms.Application.VoltageLevels.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/voltage-levels")]
public class VoltageLevelsController : ControllerBase
{
    private readonly IVoltageLevelService _voltageLevelService;

    public VoltageLevelsController(IVoltageLevelService voltageLevelService)
    {
        _voltageLevelService = voltageLevelService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _voltageLevelService.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVoltageLevelRequestDto request)
    {
        var result = await _voltageLevelService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVoltageLevelRequestDto request)
    {
        var result = await _voltageLevelService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _voltageLevelService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
