using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.OutageScheduleWindows.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.OutageScheduleWindows;

public class OutageScheduleWindowService : IOutageScheduleWindowService
{
    private readonly AppDbContext _dbContext;

    public OutageScheduleWindowService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<OutageScheduleWindowDto>>> ListAsync()
    {
        var windows = await _dbContext.OutageScheduleWindows
            .Select(w => Map(w))
            .ToListAsync();

        return ApiResponse<List<OutageScheduleWindowDto>>.Ok(windows);
    }

    public async Task<ApiResponse<List<OutageScheduleWindowDto>>> SaveAsync(SaveScheduleWindowsRequestDto request)
    {
        if (request.Windows.Any(w => w.Month is < 1 or > 12))
        {
            return ApiResponse<List<OutageScheduleWindowDto>>.Fail("Month must be between 1 and 12.");
        }

        var existing = await _dbContext.OutageScheduleWindows.ToListAsync();
        _dbContext.OutageScheduleWindows.RemoveRange(existing);

        var entities = request.Windows
            .GroupBy(w => (w.WorkTypeCode, w.OutageTypeCode, w.Month))
            .Select(g => g.First())
            .Select(w => new OutageScheduleWindow
            {
                WorkTypeCode = w.WorkTypeCode,
                OutageTypeCode = w.OutageTypeCode,
                Month = w.Month,
                IsAllowed = w.IsAllowed
            })
            .ToList();

        _dbContext.OutageScheduleWindows.AddRange(entities);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<List<OutageScheduleWindowDto>>.Ok(entities.Select(Map).ToList());
    }

    private static OutageScheduleWindowDto Map(OutageScheduleWindow window)
    {
        return new OutageScheduleWindowDto
        {
            WorkTypeCode = window.WorkTypeCode,
            OutageTypeCode = window.OutageTypeCode,
            Month = window.Month,
            IsAllowed = window.IsAllowed
        };
    }
}
