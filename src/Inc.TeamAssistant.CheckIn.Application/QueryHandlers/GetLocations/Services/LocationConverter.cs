using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;

internal sealed class LocationConverter
{
    private readonly ILogger<GetLocationsQueryHandler> _logger;
    private readonly IGeoService _geoService;

    public LocationConverter(ILogger<GetLocationsQueryHandler> logger, IGeoService geoService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _geoService = geoService ?? throw new ArgumentNullException(nameof(geoService));
    }

    public IReadOnlyCollection<LocationDto> Convert(IEnumerable<LocationOnMap> locations)
    {
        ArgumentNullException.ThrowIfNull(locations);

        return locations
            .OrderByDescending(i => i.Created)
            .Select(Convert)
            .ToArray();
    }
    
    private LocationDto Convert(LocationOnMap location)
    {
        ArgumentNullException.ThrowIfNull(location);
        
        const string unknown = "?";

        try
        {
            var country = _geoService.FindCountry(location.Latitude, location.Longitude);
            var timeZone = _geoService.GetTimeZone(location.Latitude, location.Longitude);
            
            return new(
                location.UserId,
                location.DisplayName,
                location.Longitude,
                location.Latitude,
                $"UTC {(timeZone.BaseUtcOffset < TimeSpan.Zero ? "-" : "+")}{timeZone.BaseUtcOffset:hh\\:mm}",
                country?.Name ?? unknown);
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
                CountryName: unknown);
        }
    }
}