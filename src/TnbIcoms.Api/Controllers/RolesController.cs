using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Lookups;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _roleService.ListAsync();
        return Ok(result);
    }
}
