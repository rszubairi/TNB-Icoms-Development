using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Mnemonics;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/mnemonic")]
public class MnemonicController : ControllerBase
{
    private readonly IMnemonicService _mnemonicService;

    public MnemonicController(IMnemonicService mnemonicService)
    {
        _mnemonicService = mnemonicService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _mnemonicService.ListAsync();
        return Ok(result);
    }

    [HttpPost]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { success = false, error = "A file is required." });
        }

        await using var stream = file.OpenReadStream();
        var result = await _mnemonicService.UploadAsync(stream, file.FileName, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("current/download")]
    public async Task<IActionResult> DownloadCurrent()
    {
        var (content, fileName) = await _mnemonicService.OpenCurrentAsync();
        if (content is null || fileName is null)
        {
            return NotFound(new { success = false, error = "No Mnemonic list has been uploaded yet." });
        }

        return File(content, "application/pdf", fileName);
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var (content, fileName) = await _mnemonicService.OpenAsync(id);
        if (content is null || fileName is null)
        {
            return NotFound(new { success = false, error = "File not found." });
        }

        return File(content, "application/pdf", fileName);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
