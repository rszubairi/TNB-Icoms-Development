using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.ChangeRequests;
using TnbIcoms.Application.ChangeRequests.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/change-requests")]
public class ChangeRequestsController : ControllerBase
{
    private readonly IChangeRequestService _service;

    public ChangeRequestsController(IChangeRequestService service)
    {
        _service = service;
    }

    [HttpGet("by-outage/{outageId:int}")]
    public async Task<IActionResult> ListForOutage(int outageId)
    {
        var result = await _service.ListForOutageAsync(outageId);
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> ListPending()
    {
        var result = await _service.ListPendingAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChangeRequestBatchDto request)
    {
        var result = await _service.CreateAsync(request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{batchId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid batchId)
    {
        var result = await _service.ApproveAsync(batchId, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{batchId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid batchId, [FromBody] RejectChangeRequestBatchDto request)
    {
        var result = await _service.RejectAsync(batchId, GetCurrentUserId(), request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
