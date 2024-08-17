using GeoTimeZone;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;

internal sealed class LocationConverter
{
    private readonly ILogger<GetLocationsQueryHandler> _logger;

    public LocationConverter(ILogger<GetLocationsQueryHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public LocationDto Convert(LocationOnMap location)
    {
        ArgumentNullException.ThrowIfNull(location);

        try
        {
            var timeZoneId = TimeZoneLookup.GetTimeZone(location.Latitude, location.Longitude).Result;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            
            return new(
                location.DisplayName,
                location.Longitude,
                location.Latitude,
                timeZone.BaseUtcOffset,
                $"{(timeZone.BaseUtcOffset < TimeSpan.Zero ? "-" : "+")}{timeZone.BaseUtcOffset:hh\\:mm}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Can not detect time zone for location (Latitude: {Latitude}, Longitude: {Longitude})",
                location.Latitude,
                location.Longitude);
            
            return new(
                location.DisplayName,
                location.Longitude,
                location.Latitude,
                UtcOffset: null,
                DisplayOffset: "?");
        }
    }
}