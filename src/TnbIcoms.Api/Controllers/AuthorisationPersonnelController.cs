using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.AuthorisationPersonnel;
using TnbIcoms.Application.AuthorisationPersonnel.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/authorisation-personnel")]
public class AuthorisationPersonnelController : ControllerBase
{
    private readonly IAuthorisationPersonnelService _personnelService;

    public AuthorisationPersonnelController(IAuthorisationPersonnelService personnelService)
    {
        _personnelService = personnelService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? zoneId)
    {
        var result = await _personnelService.ListAsync(zoneId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveAuthorisationPersonnelRequestDto request)
    {
        var result = await _personnelService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveAuthorisationPersonnelRequestDto request)
    {
        var result = await _personnelService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _personnelService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
