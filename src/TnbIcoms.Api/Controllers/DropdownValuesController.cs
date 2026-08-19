using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.DropdownValues;
using TnbIcoms.Application.DropdownValues.Dtos;
using TnbIcoms.Application.Lookups;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/dropdown-values")]
public class DropdownValuesController : ControllerBase
{
    private readonly IDropdownValueService _dropdownValueService;
    private readonly IDropdownValueAdminService _adminService;

    public DropdownValuesController(IDropdownValueService dropdownValueService, IDropdownValueAdminService adminService)
    {
        _dropdownValueService = dropdownValueService;
        _adminService = adminService;
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

    [HttpGet("categories")]
    public IActionResult ListCategories()
    {
        var categories = DropdownCategories.Categories
            .Select(c => new { code = c.Code, label = c.Label, hasParent = c.HasParent })
            .ToList();

        return Ok(new { success = true, data = new { categories, outageTypeParents = DropdownCategories.OutageTypeParents } });
    }

    [HttpGet("admin")]
    public async Task<IActionResult> ListForAdmin([FromQuery] string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return BadRequest(new { success = false, error = "category is required." });
        }

        var result = await _adminService.ListAsync(category);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDropdownValueRequestDto request)
    {
        var result = await _adminService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDropdownValueRequestDto request)
    {
        var result = await _adminService.UpdateAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/reorder")]
    public async Task<IActionResult> Reorder(int id, [FromBody] ReorderDropdownValueRequestDto request)
    {
        var result = await _adminService.ReorderAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var result = await _adminService.DeactivateAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
