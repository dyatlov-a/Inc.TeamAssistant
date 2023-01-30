using GeoTimeZone;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Convertors;

internal sealed class LocationConvertor
{
    private readonly ILogger<LocationConvertor> _logger;

    public LocationConvertor(ILogger<LocationConvertor> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public LocationDto ConvertFrom(LocationOnMap location)
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));

        TimeSpan? baseUtcOffset = null;

        try
        {
            var timeZoneId = TimeZoneLookup.GetTimeZone(location.Latitude, location.Longitude).Result;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            baseUtcOffset = timeZone.BaseUtcOffset;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Can not detect time zone for location (Latitude: {Latitude}, Longitude: {Longitude})",
                location.Latitude,
                location.Longitude);
        }

        return new(location.DisplayName, location.Longitude, location.Latitude, baseUtcOffset);
    }
}