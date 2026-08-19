namespace TnbIcoms.Application.SystemSettings.Dtos;

public class SystemSettingDto
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

public class SaveSystemSettingRequestDto
{
    public string SettingValue { get; set; } = string.Empty;
}
