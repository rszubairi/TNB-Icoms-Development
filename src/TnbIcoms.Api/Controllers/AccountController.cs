using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Account;
using TnbIcoms.Application.Account.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var result = await _accountService.GetProfileAsync(GetCurrentUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateAccountProfileRequestDto request)
    {
        var result = await _accountService.UpdateProfileAsync(GetCurrentUserId(), request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var result = await _accountService.ChangePasswordAsync(GetCurrentUserId(), request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
