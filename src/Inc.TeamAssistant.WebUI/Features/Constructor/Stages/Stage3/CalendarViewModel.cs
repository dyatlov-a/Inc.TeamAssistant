using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages.Stage3;

public sealed record CalendarViewModel(
    int ClientTimezoneOffset,
    GetCalendarByOwnerResult Calendar)
    : IWithEmpty<CalendarViewModel>
{
    public static CalendarViewModel Empty { get; } = new(0, GetCalendarByOwnerResult.Empty);
}