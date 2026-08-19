using TnbIcoms.Application.Common;
using TnbIcoms.Application.SystemSettings.Dtos;

namespace TnbIcoms.Application.SystemSettings;

public static class SystemSettingKeys
{
    /// <summary>Days from outage start a Requestor may submit a Change Request before it is pushed to KIV.</summary>
    public const string ChangeRequestWindowDays = "ChangeRequestWindowDays";
}

public interface ISystemSettingService
{
    Task<ApiResponse<SystemSettingDto>> GetAsync(string key, string defaultValue);
    Task<ApiResponse<SystemSettingDto>> SaveAsync(string key, SaveSystemSettingRequestDto request);
}
