namespace TnbIcoms.Application.Roles.Dtos;

public class RolePermissionDto
{
    public string ModuleCode { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty;
    public bool IsGranted { get; set; }
}

public class RoleListItemDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public bool IsActive { get; set; }
    public int PermissionCount { get; set; }
}

public class RoleDetailDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<RolePermissionDto> Permissions { get; set; } = new();
}

public class CreateRoleRequestDto
{
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public List<RolePermissionDto> Permissions { get; set; } = new();
}

public class UpdateRoleRequestDto
{
    public string RoleName { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public bool IsActive { get; set; } = true;
    public List<RolePermissionDto> Permissions { get; set; } = new();
}
