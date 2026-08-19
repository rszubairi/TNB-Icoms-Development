using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.Lookups;

public class ZoneService : IZoneService
{
    private readonly AppDbContext _dbContext;

    public ZoneService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<ZoneLookupDto>>> ListAsync()
    {
        var zones = await _dbContext.Zones
            .Where(z => z.IsActive)
            .OrderBy(z => z.ZoneName)
            .Select(z => new ZoneLookupDto
            {
                ZoneId = z.ZoneId,
                ZoneName = z.ZoneName,
                ZoneAbbr = z.ZoneAbbr
            })
            .ToListAsync();

        return ApiResponse<List<ZoneLookupDto>>.Ok(zones);
    }
}
