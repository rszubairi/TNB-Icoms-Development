using Microsoft.EntityFrameworkCore;
using TnbIcoms.Application.Common;
using TnbIcoms.Application.SystemSettings.Dtos;
using TnbIcoms.Domain.Entities.Config;
using TnbIcoms.Infrastructure.Persistence;

namespace TnbIcoms.Application.SystemSettings;

public class SystemSettingService : ISystemSettingService
{
    private readonly AppDbContext _dbContext;

    public SystemSettingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<SystemSettingDto>> GetAsync(string key, string defaultValue)
    {
        var setting = await _dbContext.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);

        return ApiResponse<SystemSettingDto>.Ok(new SystemSettingDto
        {
            SettingKey = key,
            SettingValue = setting?.SettingValue ?? defaultValue,
            UpdatedAt = setting?.UpdatedAt ?? DateTime.MinValue
        });
    }

    public async Task<ApiResponse<SystemSettingDto>> SaveAsync(string key, SaveSystemSettingRequestDto request)
    {
        var setting = await _dbContext.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);

        if (setting is null)
        {
            setting = new SystemSetting { SettingKey = key };
            _dbContext.SystemSettings.Add(setting);
        }

        setting.SettingValue = request.SettingValue;
        setting.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return ApiResponse<SystemSettingDto>.Ok(new SystemSettingDto
        {
            SettingKey = setting.SettingKey,
            SettingValue = setting.SettingValue,
            UpdatedAt = setting.UpdatedAt
        });
    }
}
