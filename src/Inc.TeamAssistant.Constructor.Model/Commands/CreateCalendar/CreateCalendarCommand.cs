using Inc.TeamAssistant.Constructor.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.CreateCalendar;

public sealed record CreateCalendarCommand(
    WorkScheduleUtcDto? Schedule,
    IReadOnlyCollection<DayOfWeek> Weekends,
    IReadOnlyDictionary<DateOnly, string> Holidays)
    : IRequest;