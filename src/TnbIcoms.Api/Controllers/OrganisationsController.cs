using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Organisations;
using TnbIcoms.Application.Organisations.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/organisations")]
public class OrganisationsController : ControllerBase
{
    private readonly IOrganisationService _organisationService;

    public OrganisationsController(IOrganisationService organisationService)
    {
        _organisationService = organisationService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? zoneId)
    {
        var result = await _organisationService.ListAsync(zoneId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganisationRequestDto request)
    {
        var result = await _organisationService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganisationRequestDto request)
    {
        var result = await _organisationService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _organisationService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
