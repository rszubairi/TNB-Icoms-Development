using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Lookups;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/dropdown-values")]
public class DropdownValuesController : ControllerBase
{
    private readonly IDropdownValueService _dropdownValueService;

    public DropdownValuesController(IDropdownValueService dropdownValueService)
    {
        _dropdownValueService = dropdownValueService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return BadRequest(new { success = false, error = "category is required." });
        }

        var result = await _dropdownValueService.ListByCategoryAsync(category);
        return Ok(result);
    }
}
