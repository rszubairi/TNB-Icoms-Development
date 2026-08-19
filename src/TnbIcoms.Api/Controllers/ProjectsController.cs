using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TnbIcoms.Application.Projects;
using TnbIcoms.Application.Projects.Dtos;

namespace TnbIcoms.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? isActive)
    {
        var result = await _projectService.ListAsync(isActive);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequestDto request)
    {
        var result = await _projectService.CreateAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, [FromBody] SetProjectStatusRequestDto request)
    {
        var result = await _projectService.SetStatusAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
