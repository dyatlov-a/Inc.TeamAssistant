using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Constructor.Model.Commands.UpdateCalendar;

public sealed record UpdateCalendarResult(Guid Id)
    : IWithEmpty<UpdateCalendarResult>
{
    public static UpdateCalendarResult Empty { get; } = new(Guid.Empty);
}