using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.OutageTypeRules;
using TnbIcoms.Application.OutageTypeRules.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/outage-type-rules")]
public class OutageTypeRulesController : ControllerBase
{
    private readonly IOutageTypeRuleService _ruleService;

    public OutageTypeRulesController(IOutageTypeRuleService ruleService)
    {
        _ruleService = ruleService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _ruleService.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveOutageTypeRuleRequestDto request)
    {
        var result = await _ruleService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveOutageTypeRuleRequestDto request)
    {
        var result = await _ruleService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _ruleService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
