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

    public IReadOnlyCollection<LocationDto> Convert(IEnumerable<LocationOnMap> locations, Calendar? calendar)
    {
        ArgumentNullException.ThrowIfNull(locations);

        return locations
            .OrderByDescending(i => i.Created)
            .Select(i => Convert(i, calendar))
            .ToArray();
    }
    
    private LocationDto Convert(LocationOnMap location, Calendar? calendar)
    {
        ArgumentNullException.ThrowIfNull(location);
        
        const string unknown = "?";
        const string defaultTimeZone = "UTC";

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
                workSchedule);

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
                WorkSchedule: unknown);
        }
    }
}