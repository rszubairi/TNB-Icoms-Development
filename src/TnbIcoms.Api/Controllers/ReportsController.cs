using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Reports;
using TnbIcoms.Application.Reports.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] ReportFilterDto filter)
    {
        var result = await _reportService.GenerateAsync(filter);
        return Ok(result);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] ReportFilterDto filter)
    {
        var bytes = await _reportService.ExportExcelAsync(filter);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"outage-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx");
    }

    [HttpPost("export/pdf")]
    public async Task<IActionResult> ExportPdf([FromBody] ReportFilterDto filter)
    {
        var bytes = await _reportService.ExportPdfAsync(filter);
        return File(bytes, "application/pdf", $"outage-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf");
    }

    [HttpGet("favourites")]
    public async Task<IActionResult> ListFavourites()
    {
        var result = await _reportService.ListFavouritesAsync(GetCurrentUserId());
        return Ok(result);
    }

    [HttpPost("favourites")]
    public async Task<IActionResult> SaveFavourite([FromBody] SaveReportFilterRequestDto request)
    {
        var result = await _reportService.SaveFavouriteAsync(GetCurrentUserId(), request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("favourites/{id:int}")]
    public async Task<IActionResult> DeleteFavourite(int id)
    {
        var result = await _reportService.DeleteFavouriteAsync(GetCurrentUserId(), id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirstValue("userId");
        return int.TryParse(claim, out var userId) ? userId : 0;
    }
}
