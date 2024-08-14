using Inc.TeamAssistant.Constructor.Model.Common;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;

public sealed record GetCalendarByOwnerResult(
    Guid Id,
    long OwnerId,
    WorkScheduleUtcDto? Schedule,
    IReadOnlyCollection<DayOfWeek> Weekends,
    IReadOnlyDictionary<DateOnly, string> Holidays);