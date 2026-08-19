using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Lookups;

public class DropdownValueService : IDropdownValueService
{
    private readonly AppDbContext _dbContext;

    public DropdownValueService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<DropdownValueLookupDto>>> ListByCategoryAsync(string category)
    {
        var values = await _dbContext.DropdownValues
            .Where(d => d.CategoryCode == category && d.IsActive)
            .OrderBy(d => d.SortOrder)
            .ThenBy(d => d.ValueLabel)
            .Select(d => new DropdownValueLookupDto
            {
                DropdownValueId = d.DropdownValueId,
                CategoryCode = d.CategoryCode,
                ValueCode = d.ValueCode,
                ValueLabel = d.ValueLabel
            })
            .ToListAsync();

        return ApiResponse<List<DropdownValueLookupDto>>.Ok(values);
    }
}
