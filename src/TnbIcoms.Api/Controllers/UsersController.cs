using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Users;
using TnbIcoms.Application.Users.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? roleId, [FromQuery] int? zoneId, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _userService.ListAsync(new UserListQuery
        {
            RoleId = roleId,
            ZoneId = zoneId,
            Search = search,
            Page = page,
            PageSize = pageSize
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequestDto request)
    {
        var result = await _userService.CreateAsync(request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequestDto request)
    {
        var result = await _userService.UpdateAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _userService.DeactivateAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
