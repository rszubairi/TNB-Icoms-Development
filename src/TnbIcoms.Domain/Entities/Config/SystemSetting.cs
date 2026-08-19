namespace TnbIcoms.Domain.Entities.Config;

/// <summary>
/// Generic single-value admin settings (e.g. Change Request submission window in days).
/// </summary>
public class SystemSetting
{
    public int SystemSettingId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
