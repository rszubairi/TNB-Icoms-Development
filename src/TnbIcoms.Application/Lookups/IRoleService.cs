using TnbIcoms.Application.Common;

namespace TnbIcoms.Application.Lookups;

public class RoleLookupDto
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleCode { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
}

public interface IRoleService
{
    Task<ApiResponse<List<RoleLookupDto>>> ListAsync();
}
