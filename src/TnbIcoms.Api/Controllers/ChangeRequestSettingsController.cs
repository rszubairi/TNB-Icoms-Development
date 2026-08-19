using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.SystemSettings;
using TnbIcoms.Application.SystemSettings.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/change-request-settings")]
public class ChangeRequestSettingsController : ControllerBase
{
    private readonly ISystemSettingService _settingService;

    public ChangeRequestSettingsController(ISystemSettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _settingService.GetAsync(SystemSettingKeys.ChangeRequestWindowDays, defaultValue: "7");
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] SaveSystemSettingRequestDto request)
    {
        if (!int.TryParse(request.SettingValue, out var days) || days < 1)
        {
            return BadRequest(new { success = false, error = "Enter a whole number of days, 1 or greater." });
        }

        var result = await _settingService.SaveAsync(SystemSettingKeys.ChangeRequestWindowDays, request);
        return Ok(result);
    }
}
