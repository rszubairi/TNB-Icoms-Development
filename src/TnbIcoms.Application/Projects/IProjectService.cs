using TnbIcoms.Application.Common;
using TnbIcoms.Application.Projects.Dtos;

namespace TnbIcoms.Application.Projects;

public interface IProjectService
{
    Task<ApiResponse<List<ProjectDto>>> ListAsync(bool? isActive);
    Task<ApiResponse<ProjectDto>> CreateAsync(CreateProjectRequestDto request);
    Task<ApiResponse<ProjectDto>> SetStatusAsync(int projectId, SetProjectStatusRequestDto request);
}
