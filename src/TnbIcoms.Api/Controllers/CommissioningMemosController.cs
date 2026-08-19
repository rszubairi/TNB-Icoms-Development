using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.CommissioningMemos;
using TnbIcoms.Application.CommissioningMemos.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/commissioning-memos")]
public class CommissioningMemosController : ControllerBase
{
    private readonly ICommissioningMemoService _memoService;

    public CommissioningMemosController(ICommissioningMemoService memoService)
    {
        _memoService = memoService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? outageId, [FromQuery] string? status)
    {
        var result = await _memoService.ListAsync(outageId, status);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _memoService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommissioningMemoRequestDto request)
    {
        var result = await _memoService.CreateAsync(request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}/cover-page.pdf")]
    public async Task<IActionResult> DownloadCoverPage(int id)
    {
        var bytes = await _memoService.GenerateCoverPagePdfAsync(id);
        if (bytes.Length == 0) return NotFound(new { success = false, error = "Commissioning Memo not found." });
        return File(bytes, "application/pdf", $"commissioning-memo-{id}.pdf");
    }

    [HttpPut("{id:int}/engineer-pic-review")]
    public async Task<IActionResult> EngineerPicReview(int id, [FromBody] MemoStageReviewRequestDto request)
    {
        var result = await _memoService.EngineerPicReviewAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/se-review")]
    public async Task<IActionResult> SeReview(int id, [FromBody] MemoStageReviewRequestDto request)
    {
        var result = await _memoService.SeReviewAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/dce-review")]
    public async Task<IActionResult> DceReview(int id, [FromBody] MemoStageReviewRequestDto request)
    {
        var result = await _memoService.DceReviewAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/ce-gnm-review")]
    public async Task<IActionResult> CeGnmReview(int id, [FromBody] MemoStageReviewRequestDto request)
    {
        var result = await _memoService.CeGnmReviewAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/final-sign-off")]
    public async Task<IActionResult> FinalSignOff(int id, [FromBody] MemoStageReviewRequestDto request)
    {
        var result = await _memoService.FinalSignOffAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/commissioning-result")]
    public async Task<IActionResult> SetCommissioningResult(int id, [FromBody] SetCommissioningResultRequestDto request)
    {
        var result = await _memoService.SetCommissioningResultAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
