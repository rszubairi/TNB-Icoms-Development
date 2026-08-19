using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Lookups;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/zones")]
public class ZonesController : ControllerBase
{
    private readonly IZoneService _zoneService;

    public ZonesController(IZoneService zoneService)
    {
        _zoneService = zoneService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _zoneService.ListAsync();
        return Ok(result);
    }
}
