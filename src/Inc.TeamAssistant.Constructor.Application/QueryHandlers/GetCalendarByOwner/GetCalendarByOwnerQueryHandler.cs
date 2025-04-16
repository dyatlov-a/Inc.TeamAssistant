using Inc.TeamAssistant.Constructor.Application.Contracts;
using Inc.TeamAssistant.Constructor.Model.Common;
using Inc.TeamAssistant.Constructor.Model.Queries.GetCalendarByOwner;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetCalendarByOwner;

internal sealed class GetCalendarByOwnerQueryHandler
    : IRequestHandler<GetCalendarByOwnerQuery, GetCalendarByOwnerResult>
{
    private readonly ICalendarRepository _repository;
    private readonly ICurrentPersonResolver _personResolver;

    public GetCalendarByOwnerQueryHandler(ICalendarRepository repository, ICurrentPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetCalendarByOwnerResult> Handle(GetCalendarByOwnerQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _personResolver.GetCurrentPerson();
        var calendar = await _repository.Find(currentPerson.Id, token);

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