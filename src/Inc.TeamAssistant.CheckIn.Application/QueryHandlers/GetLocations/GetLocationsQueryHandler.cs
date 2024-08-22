using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Converters;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations;

internal sealed class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsResult>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly LocationConverter _locationConverter;
    private readonly IHolidayReader _holidayReader;
    private readonly IEnumerable<IPersonStatsProvider> _personStatsProviders;

    public GetLocationsQueryHandler(
        ILocationsRepository locationsRepository,
        LocationConverter locationConverter,
        IHolidayReader holidayReader,
        IEnumerable<IPersonStatsProvider> personStatsProviders)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _locationConverter = locationConverter ?? throw new ArgumentNullException(nameof(locationConverter));
        _holidayReader = holidayReader ?? throw new ArgumentNullException(nameof(holidayReader));
        _personStatsProviders = personStatsProviders ?? throw new ArgumentNullException(nameof(personStatsProviders));
    }

    public async Task<GetLocationsResult> Handle(GetLocationsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var from = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(90));
        var locations = await _locationsRepository.GetLocations(query.MapId, token);
        var personIds = locations.Select(l => l.UserId).Distinct().ToArray();
        var personStatsLookup = await Build(personIds, from, token);
        var calendar = locations.Any()
            ? await _holidayReader.Find(locations.First().Map.BotId, token)
            : null;
        var locationsByUser = locations
            .GroupBy(l => l.DisplayName)
            .ToDictionary(l => l.Key, l => _locationConverter.Convert(l, personStatsLookup, calendar));
        

        return new(locationsByUser);
    }

    private async Task<IReadOnlyDictionary<string, IReadOnlyDictionary<long, int>>> Build(
        IReadOnlyCollection<long> personIds,
        DateTimeOffset from,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(personIds);
        
        var personStatsLookup = new Dictionary<string, IReadOnlyDictionary<long, int>>();

        foreach (var personStatsProvider in _personStatsProviders)
            personStatsLookup.Add(
                personStatsProvider.FeatureName,
                await personStatsProvider.GetStats(personIds, from, token));

        return personStatsLookup;
    }
}