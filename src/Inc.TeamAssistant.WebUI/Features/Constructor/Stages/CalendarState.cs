using Inc.TeamAssistant.Constructor.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed record CalendarState(
    bool WorkAllDay,
    WorkScheduleUtcDto Schedule,
    IReadOnlyCollection<DayOfWeek> Weekends,
    IReadOnlyDictionary<DateOnly, string> Holidays);