namespace TnbIcoms.Domain.Entities.Auth;

public class RolePermission
{
    public int RolePermissionId { get; set; }
    public int RoleId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string PermissionCode { get; set; } = string.Empty; // View, Create, Edit, Delete, Approve
    public bool IsGranted { get; set; } = true;

    public Role? Role { get; set; }
}
