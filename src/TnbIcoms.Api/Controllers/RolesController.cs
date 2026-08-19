using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Roles;
using TnbIcoms.Application.Roles.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleAdminService _roleService;

    public RolesController(IRoleAdminService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _roleService.ListAsync();
        return Ok(result);
    }

    [HttpGet("modules")]
    public IActionResult ListPermissionModules()
    {
        var modules = PermissionModules.Modules.Select(m => new { code = m.Code, label = m.Label }).ToList();
        return Ok(new { success = true, data = new { modules, actions = PermissionModules.StandardActions } });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _roleService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto request)
    {
        var result = await _roleService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleRequestDto request)
    {
        var result = await _roleService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _roleService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
