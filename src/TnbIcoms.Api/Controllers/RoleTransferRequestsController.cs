using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.RoleTransferRequests;
using TnbIcoms.Application.RoleTransferRequests.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/role-transfer-requests")]
public class RoleTransferRequestsController : ControllerBase
{
    private readonly IRoleTransferRequestService _service;

    public RoleTransferRequestsController(IRoleTransferRequestService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _service.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleTransferRequestDto request)
    {
        var result = await _service.CreateAsync(GetCurrentUserId(), request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var result = await _service.ApproveAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectRoleTransferRequestDto request)
    {
        var result = await _service.RejectAsync(id, GetCurrentUserId(), request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
