using TnbIcoms.Application.Common;

namespace TnbIcoms.Application.Lookups;

public class DropdownValueLookupDto
{
    public int DropdownValueId { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string ValueCode { get; set; } = string.Empty;
    public string ValueLabel { get; set; } = string.Empty;
}

public interface IDropdownValueService
{
    Task<ApiResponse<List<DropdownValueLookupDto>>> ListByCategoryAsync(string category);
}
