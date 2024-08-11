using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage4;

public sealed record CalendarViewModel(
    string MoveNextTitle)
    : IViewModel<CalendarViewModel>
{
    public static CalendarViewModel Empty { get; } = new(
        string.Empty);
}