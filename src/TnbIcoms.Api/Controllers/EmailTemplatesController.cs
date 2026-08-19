using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.EmailTemplates;
using TnbIcoms.Application.EmailTemplates.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/email-templates")]
public class EmailTemplatesController : ControllerBase
{
    private readonly IEmailTemplateService _emailTemplateService;

    public EmailTemplatesController(IEmailTemplateService emailTemplateService)
    {
        _emailTemplateService = emailTemplateService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _emailTemplateService.ListAsync();
        return Ok(result);
    }

    [HttpGet("{templateCode}")]
    public async Task<IActionResult> GetByCode(string templateCode)
    {
        var result = await _emailTemplateService.GetByCodeAsync(templateCode);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPut("{templateCode}")]
    public async Task<IActionResult> Update(string templateCode, [FromBody] UpdateEmailTemplateRequestDto request)
    {
        var result = await _emailTemplateService.UpdateAsync(templateCode, request, GetCurrentUserId());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
