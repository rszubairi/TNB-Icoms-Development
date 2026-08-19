using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Outages;
using TnbIcoms.Application.Outages.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/outages")]
public class OutagesController : ControllerBase
{
    private readonly IOutageService _outageService;

    public OutagesController(IOutageService outageService)
    {
        _outageService = outageService;
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int? zoneId,
        [FromQuery] string? requestorStatus,
        [FromQuery] string? gnmStatus,
        [FromQuery] string? outageCode,
        [FromQuery] bool pendingPlannerReviewOnly = false,
        [FromQuery] bool pendingConfirmationOnly = false,
        [FromQuery] bool agreedAndConfirmedOnly = false,
        [FromQuery] bool pendingGnmApprovalOnly = false)
    {
        var result = await _outageService.ListAsync(new OutageListFilter
        {
            ZoneId = zoneId,
            RequestorStatus = requestorStatus,
            GnmStatus = gnmStatus,
            OutageCode = outageCode,
            PendingPlannerReviewOnly = pendingPlannerReviewOnly,
            PendingConfirmationOnly = pendingConfirmationOnly,
            AgreedAndConfirmedOnly = agreedAndConfirmedOnly,
            PendingGnmApprovalOnly = pendingGnmApprovalOnly
        });
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _outageService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOutageRequestDto request)
    {
        var result = await _outageService.CreateAsync(request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/submit")]
    public async Task<IActionResult> SubmitDraft(int id)
    {
        var result = await _outageService.SubmitDraftAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/agree")]
    public async Task<IActionResult> Agree(int id)
    {
        var result = await _outageService.AgreeAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/disagree")]
    public async Task<IActionResult> Disagree(int id)
    {
        var result = await _outageService.DisagreeAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        var result = await _outageService.ConfirmAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var result = await _outageService.RejectAsync(id, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("bulk/agree")]
    public async Task<IActionResult> BulkAgree([FromBody] BulkActionRequestDto request)
    {
        var result = await _outageService.BulkAgreeAsync(request, GetCurrentUserId());
        return Ok(result);
    }

    [HttpPost("bulk/disagree")]
    public async Task<IActionResult> BulkDisagree([FromBody] BulkActionRequestDto request)
    {
        var result = await _outageService.BulkDisagreeAsync(request, GetCurrentUserId());
        return Ok(result);
    }

    [HttpPost("bulk/confirm")]
    public async Task<IActionResult> BulkConfirm([FromBody] BulkActionRequestDto request)
    {
        var result = await _outageService.BulkConfirmAsync(request, GetCurrentUserId());
        return Ok(result);
    }

    [HttpPost("bulk/reject")]
    public async Task<IActionResult> BulkReject([FromBody] BulkActionRequestDto request)
    {
        var result = await _outageService.BulkRejectAsync(request, GetCurrentUserId());
        return Ok(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
