namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed class HolidayFromModel
{
    public string DateFieldId { get; set; } = default!;
    public string WorkdayFieldId { get; set; } = default!;
    public DateOnly Date { get; set; }
    public bool IsWorkday { get; set; }
}