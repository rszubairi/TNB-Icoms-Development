using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.Projects.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Projects;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _dbContext;

    public ProjectService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<ProjectDto>>> ListAsync(bool? isActive)
    {
        var query = _dbContext.Projects
            .Include(p => p.Zone)
            .AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var projects = await query
            .OrderByDescending(p => p.IsActive)
            .ThenByDescending(p => p.ProjectId)
            .ToListAsync();

        var openCounts = await GetOpenOutageCountsAsync(projects.Select(p => p.ProjectId));

        return ApiResponse<List<ProjectDto>>.Ok(projects.Select(p => Map(p, openCounts)).ToList());
    }

    public async Task<ApiResponse<ProjectDto>> CreateAsync(CreateProjectRequestDto request)
    {
        var tpCode = request.TpCode.Trim();
        var suffix = request.ProjectSuffix.Trim();

        if (string.IsNullOrWhiteSpace(tpCode) || string.IsNullOrWhiteSpace(suffix))
        {
            return ApiResponse<ProjectDto>.Fail("TP Code and Name are both required.");
        }

        if (await _dbContext.Projects.AnyAsync(p => p.TpCode == tpCode))
        {
            return ApiResponse<ProjectDto>.Fail("This TP Code is already in use. TP Codes cannot be reused.");
        }

        var project = new Project
        {
            TpCode = tpCode,
            ProjectSuffix = suffix,
            ProjectName = $"{tpCode} - {suffix}",
            ZoneId = request.ZoneId,
            IsActive = true
        };

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync();

        if (request.ZoneId.HasValue)
        {
            await _dbContext.Entry(project).Reference(p => p.Zone).LoadAsync();
        }

        return ApiResponse<ProjectDto>.Ok(Map(project, new Dictionary<int, int>()));
    }

    public async Task<ApiResponse<ProjectDto>> SetStatusAsync(int projectId, SetProjectStatusRequestDto request)
    {
        var project = await _dbContext.Projects
            .Include(p => p.Zone)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);

        if (project is null)
        {
            return ApiResponse<ProjectDto>.Fail("Project not found.");
        }

        project.IsActive = request.IsActive;
        project.EndDate = request.IsActive ? null : DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var openCounts = await GetOpenOutageCountsAsync(new[] { projectId });

        return ApiResponse<ProjectDto>.Ok(Map(project, openCounts));
    }

    /// <summary>
    /// URS Module 1 §5.2.7: an outage is "open" while it hasn't reached any Outage Closed* status
    /// across the Requestor, GNM, or GNC status columns.
    /// </summary>
    private async Task<Dictionary<int, int>> GetOpenOutageCountsAsync(IEnumerable<int> projectIds)
    {
        var ids = projectIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<int, int>();
        }

        return await _dbContext.Outages
            .Where(o => o.ProjectId.HasValue && ids.Contains(o.ProjectId.Value))
            .Where(o => !o.RequestorStatus.StartsWith("Outage Closed")
                        && !o.GnmStatus.StartsWith("Outage Closed")
                        && (o.GncStatus == null || !o.GncStatus.StartsWith("Outage Closed")))
            .GroupBy(o => o.ProjectId!.Value)
            .Select(g => new { ProjectId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.ProjectId, g => g.Count);
    }

    private static ProjectDto Map(Project project, Dictionary<int, int> openCounts)
    {
        return new ProjectDto
        {
            ProjectId = project.ProjectId,
            TpCode = project.TpCode,
            ProjectSuffix = project.ProjectSuffix,
            ProjectName = project.ProjectName,
            ZoneId = project.ZoneId,
            ZoneName = project.Zone?.ZoneName,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsActive = project.IsActive,
            OpenOutageCount = openCounts.TryGetValue(project.ProjectId, out var count) ? count : 0
        };
    }
}
