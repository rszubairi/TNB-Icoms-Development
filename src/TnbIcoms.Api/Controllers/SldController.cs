using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.SingleLineDiagrams;
using TnbIcoms.Application.SingleLineDiagrams.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/sld")]
public class SldController : ControllerBase
{
    private readonly ISldService _sldService;

    public SldController(ISldService sldService)
    {
        _sldService = sldService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int? stationId, [FromQuery] string? status)
    {
        var result = await _sldService.ListAsync(stationId, status);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _sldService.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSldRequestDto request)
    {
        var result = await _sldService.CreateAsync(request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/drawing")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> UploadDrawing(int id, IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { success = false, error = "A file is required." });
        }

        await using var stream = file.OpenReadStream();
        var result = await _sldService.UploadDrawingAsync(id, stream, file.FileName, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}/drawing/download")]
    public async Task<IActionResult> DownloadDrawing(int id)
    {
        var (content, fileName) = await _sldService.OpenDrawingAsync(id);
        if (content is null || fileName is null)
        {
            return NotFound(new { success = false, error = "No drawing has been uploaded for this diagram." });
        }

        return File(content, "application/octet-stream", fileName);
    }

    [HttpPut("{id:int}/engineer-review")]
    public async Task<IActionResult> EngineerReview(int id, [FromBody] EngineerReviewRequestDto request)
    {
        var result = await _sldService.EngineerReviewAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/se-review")]
    public async Task<IActionResult> SeReview(int id, [FromBody] StageReviewRequestDto request)
    {
        var result = await _sldService.SeReviewAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/dce-review")]
    public async Task<IActionResult> DceReview(int id, [FromBody] StageReviewRequestDto request)
    {
        var result = await _sldService.DceReviewAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/requestor-approve")]
    public async Task<IActionResult> RequestorApprove(int id, [FromBody] StageReviewRequestDto request)
    {
        var result = await _sldService.RequestorApproveAsync(id, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
