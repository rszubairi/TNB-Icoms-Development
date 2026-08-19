namespace TnbIcoms.Application.Projects.Dtos;

public class ProjectDto
{
    public int ProjectId { get; set; }
    public string TpCode { get; set; } = string.Empty;
    public string ProjectSuffix { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public int? ZoneId { get; set; }
    public string? ZoneName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public int OpenOutageCount { get; set; }
}

public class CreateProjectRequestDto
{
    public string TpCode { get; set; } = string.Empty;
    public string ProjectSuffix { get; set; } = string.Empty;
    public int? ZoneId { get; set; }
}

public class SetProjectStatusRequestDto
{
    public bool IsActive { get; set; }
}
