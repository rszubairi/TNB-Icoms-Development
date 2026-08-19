namespace TnbIcoms.Application.OutageScheduleWindows.Dtos;

public class OutageScheduleWindowDto
{
    public string WorkTypeCode { get; set; } = string.Empty;
    public string OutageTypeCode { get; set; } = string.Empty;
    public int Month { get; set; }
    public bool IsAllowed { get; set; }
}

public class SaveScheduleWindowsRequestDto
{
    public List<OutageScheduleWindowDto> Windows { get; set; } = new();
}
