using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Converters;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.Holidays;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations;

internal sealed class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsResult>
{
    private readonly ILocationsRepository _repository;
    private readonly LocationConverter _converter;
    private readonly IHolidayReader _reader;
    private readonly StatsByPersonBuilder _builder;

    public GetLocationsQueryHandler(
        ILocationsRepository locationsRepository,
        LocationConverter locationConverter,
        IHolidayReader holidayReader,
        StatsByPersonBuilder builder)
    {
        _repository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _converter = locationConverter ?? throw new ArgumentNullException(nameof(locationConverter));
        _reader = holidayReader ?? throw new ArgumentNullException(nameof(holidayReader));
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    public async Task<GetLocationsResult> Handle(GetLocationsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var from = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(90));
        var locations = await _repository.GetLocations(query.MapId, token);
        var personIds = locations.Select(l => l.UserId).Distinct().ToArray();
        var personStatsLookup = await _builder.Build(personIds, from, token);
        var calendar = locations.Any()
            ? await _reader.Find(locations.First().Map.BotId, token)
            : null;
        var locationsByUser = locations
            .GroupBy(l => l.DisplayName)
            .ToDictionary(l => l.Key, l => _converter.Convert(l, personStatsLookup, calendar));

        return new(locationsByUser);
    }
}