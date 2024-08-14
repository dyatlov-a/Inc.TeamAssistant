using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed record CalendarViewModel(
    string WorkTimeTitle,
    string WorkTimeHelp,
    string WorkAllDayLabel,
    string WorkTimeStartLabel,
    string WorkTimeEndLabel,
    string WeekendsTitle,
    string WeekendsHelp,
    string WeekendsLabel,
    string HolidaysTitle,
    string HolidaysHelp,
    string DateLabel,
    string WorkdayLabel,
    string AddHolidayLabel,
    string MoveNextTitle)
    : IViewModel<CalendarViewModel>
{
    public static CalendarViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}