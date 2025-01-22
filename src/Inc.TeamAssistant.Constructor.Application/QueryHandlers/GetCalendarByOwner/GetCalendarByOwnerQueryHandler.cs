using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetCalendarByOwner;

internal sealed class GetCalendarByOwnerQueryHandler
    : IRequestHandler<GetCalendarByOwnerQuery, GetCalendarByOwnerResult>
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly ICurrentPersonResolver _currentPersonResolver;

    public GetCalendarByOwnerQueryHandler(
        ICalendarRepository calendarRepository,
        ICurrentPersonResolver currentPersonResolver)
    {
        _calendarRepository = calendarRepository ?? throw new ArgumentNullException(nameof(calendarRepository));
        _currentPersonResolver =
            currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
    }

    public async Task<GetCalendarByOwnerResult> Handle(GetCalendarByOwnerQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _currentPersonResolver.GetCurrentPerson();
        var calendar = await _calendarRepository.FindByOwner(currentPerson.Id, token);

        return calendar is null
            ? GetCalendarByOwnerResult.Empty
            : new GetCalendarByOwnerResult(calendar.Id, calendar.OwnerId,
                calendar.Schedule is null
                    ? null
                    : new WorkScheduleUtcDto(calendar.Schedule.Start, calendar.Schedule.End),
                calendar.Weekends,
                calendar.Holidays.ToDictionary(h => h.Key, h => h.Value.ToString()));
    }
}