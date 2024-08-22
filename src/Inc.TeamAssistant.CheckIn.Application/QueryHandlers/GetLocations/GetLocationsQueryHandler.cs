using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Converters;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.Holidays;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations;

internal sealed class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsResult>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly LocationConverter _locationConverter;
    private readonly IHolidayReader _holidayReader;

    public GetLocationsQueryHandler(
        ILocationsRepository locationsRepository,
        LocationConverter locationConverter,
        IHolidayReader holidayReader)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _locationConverter = locationConverter ?? throw new ArgumentNullException(nameof(locationConverter));
        _holidayReader = holidayReader ?? throw new ArgumentNullException(nameof(holidayReader));
    }

    public async Task<GetLocationsResult> Handle(GetLocationsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var locations = await _locationsRepository.GetLocations(query.MapId, token);
        var calendar = locations.Any()
            ? await _holidayReader.Find(locations.First().Map.BotId, token)
            : null;
        var locationsByUser = locations
            .GroupBy(l => l.DisplayName)
            .ToDictionary(l => l.Key, l => _locationConverter.Convert(l, calendar));

        return new(locationsByUser);
    }
}