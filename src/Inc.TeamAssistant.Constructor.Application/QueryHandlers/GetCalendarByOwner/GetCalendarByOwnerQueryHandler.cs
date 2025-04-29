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
    private readonly IPersonResolver _personResolver;

    public GetCalendarByOwnerQueryHandler(ICalendarRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetCalendarByOwnerResult> Handle(GetCalendarByOwnerQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var currentPerson = _personResolver.GetCurrentPerson();
        var calendar = await _repository.Find(currentPerson.Id, token);

        if (calendar is null)
            return GetCalendarByOwnerResult.Empty;

        var now = DateTimeOffset.UtcNow;
        var schedule = calendar.Schedule is null
            ? null
            : new WorkScheduleUtcDto(calendar.Schedule.Start, calendar.Schedule.End);
        var futureHolidays = calendar.Holidays
            .Where(c => c.Key.Year >= now.Year)
            .ToDictionary(h => h.Key, h => h.Value.ToString());
        
        var result = new GetCalendarByOwnerResult(
            calendar.Id,
            calendar.OwnerId,
            schedule,
            calendar.Weekends,
            futureHolidays);
        return result;
    }
}