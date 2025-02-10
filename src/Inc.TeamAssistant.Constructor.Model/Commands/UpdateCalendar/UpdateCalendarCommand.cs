using Inc.TeamAssistant.Constructor.Model.Common;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;

public sealed record UpdateCalendarCommand(
    WorkScheduleUtcDto? Schedule,
    IReadOnlyCollection<DayOfWeek> Weekends,
    IReadOnlyDictionary<DateOnly, string> Holidays)
    : IRequest<UpdateCalendarResult>;