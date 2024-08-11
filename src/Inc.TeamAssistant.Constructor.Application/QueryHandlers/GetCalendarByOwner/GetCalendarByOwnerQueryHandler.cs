using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetCalendarByOwner;

internal sealed class GetCalendarByOwnerQueryHandler
    : IRequestHandler<GetCalendarByOwnerQuery, GetCalendarByOwnerResult?>
{
    private readonly ICalendarRepository _calendarRepository;

    public GetCalendarByOwnerQueryHandler(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository ?? throw new ArgumentNullException(nameof(calendarRepository));
    }

    public async Task<GetCalendarByOwnerResult?> Handle(GetCalendarByOwnerQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var calendar = await _calendarRepository.FindByOwner(query.OwnerId, token);

        return calendar is null
            ? null
            : new GetCalendarByOwnerResult(calendar.Id, calendar.OwnerId,
                calendar.Schedule is null
                    ? null
                    : new WorkScheduleUtcDto(calendar.Schedule.Start, calendar.Schedule.End),
                calendar.Weekends,
                calendar.Holidays.ToDictionary(h => h.Key, h => h.Value.ToString()));
    }
}