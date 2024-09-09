namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed class HolidayFromModel
{
    public string DateFieldId { get; set; } = string.Empty;
    public string WorkdayFieldId { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public bool IsWorkday { get; set; }
}