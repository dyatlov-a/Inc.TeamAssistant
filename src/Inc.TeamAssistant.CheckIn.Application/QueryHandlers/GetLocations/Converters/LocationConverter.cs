using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Inc.TeamAssistant.Holidays.Model;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Converters;

internal sealed class LocationConverter
{
    private readonly ILogger<GetLocationsQueryHandler> _logger;
    private readonly IGeoService _geoService;

    public LocationConverter(ILogger<GetLocationsQueryHandler> logger, IGeoService geoService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _geoService = geoService ?? throw new ArgumentNullException(nameof(geoService));
    }

    public IReadOnlyCollection<LocationDto> Convert(
        IEnumerable<LocationOnMap> locations,
        IReadOnlyDictionary<string, IReadOnlyDictionary<long, int>> personStatsLookup,
        Calendar? calendar)
    {
        ArgumentNullException.ThrowIfNull(locations);
        ArgumentNullException.ThrowIfNull(personStatsLookup);
        
        var maxStatsLookup = personStatsLookup
            .ToDictionary(i => i.Key, i => i.Value.Values.Any() ? i.Value.Values.Max() : 0);

        return locations
            .OrderByDescending(i => i.Created)
            .Select(i => Convert(i, personStatsLookup, maxStatsLookup, calendar))
            .ToArray();
    }
    
    private LocationDto Convert(
        LocationOnMap location,
        IReadOnlyDictionary<string, IReadOnlyDictionary<long, int>> personStatsLookup,
        IReadOnlyDictionary<string, int> maxStatsLookup,
        Calendar? calendar)
    {
        ArgumentNullException.ThrowIfNull(location);
        ArgumentNullException.ThrowIfNull(personStatsLookup);
        ArgumentNullException.ThrowIfNull(maxStatsLookup);
        
        const string unknown = "?";
        const string defaultTimeZone = "UTC";

        var stats = personStatsLookup.Keys
            .Select(k => new PersonStats(k, maxStatsLookup[k] == 0
                ? 0
                : Calculate(location.UserId, maxStatsLookup[k], personStatsLookup[k])))
            .ToArray();

        try
        {
            var country = _geoService.FindCountry(location.Latitude, location.Longitude);
            var timeZone = _geoService.GetTimeZone(location.Latitude, location.Longitude);
            var workSchedule = calendar?.Schedule is null
                ? defaultTimeZone 
                : $"{ToLocalTime(calendar.Schedule.Start)}-{ToLocalTime(calendar.Schedule.End)}";
            
            return new(
                location.UserId,
                location.DisplayName,
                location.Longitude,
                location.Latitude,
                $"{(timeZone.BaseUtcOffset < TimeSpan.Zero ? "-" : "+")}{timeZone.BaseUtcOffset:hh\\:mm}",
                country?.Name ?? unknown,
                workSchedule,
                stats);

            string ToLocalTime(TimeOnly value) => value.Add(timeZone.BaseUtcOffset).ToString("HH:mm");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Can not detect time zone for location (Latitude: {Latitude}, Longitude: {Longitude})",
                location.Latitude,
                location.Longitude);
            
            return new(
                location.UserId,
                location.DisplayName,
                location.Longitude,
                location.Latitude,
                DisplayTimeOffset: unknown,
                CountryName: unknown,
                WorkSchedule: unknown,
                stats);
        }
    }

    private static int Calculate(long personId, int maxValue, IReadOnlyDictionary<long, int> personStats)
    {
        ArgumentNullException.ThrowIfNull(personStats);
        
        const decimal weight = .2m;

        var value = (decimal)personStats.GetValueOrDefault(personId, 0);
        var result = value / maxValue / weight;

        return (int)result;
    }
}