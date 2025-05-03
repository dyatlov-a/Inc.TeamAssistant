using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

public sealed record GetCalendarByOwnerResult(
    Guid Id,
    long OwnerId,
    WorkScheduleUtcDto? Schedule,
    IReadOnlyCollection<DayOfWeek> Weekends,
    IReadOnlyDictionary<DateOnly, string> Holidays)
    : IWithEmpty<GetCalendarByOwnerResult>
{
    public static GetCalendarByOwnerResult Empty { get; } = new(
        Guid.Empty,
        0,
        null,
        [],
        new Dictionary<DateOnly, string>());
}