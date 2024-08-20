using GeoTimeZone;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;

internal sealed class LocationConverter
{
    private readonly ILogger<GetLocationsQueryHandler> _logger;
    private readonly IReverseLookup _reverseLookup;

    public LocationConverter(ILogger<GetLocationsQueryHandler> logger, IReverseLookup reverseLookup)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _reverseLookup = reverseLookup ?? throw new ArgumentNullException(nameof(reverseLookup));
    }
    
    public LocationDto Convert(LocationOnMap location)
    {
        ArgumentNullException.ThrowIfNull(location);
        
        const string unknown = "?";

        try
        {
            var timeZoneId = TimeZoneLookup.GetTimeZone(location.Latitude, location.Longitude);
            var country = _reverseLookup.Lookup((float)location.Latitude, (float)location.Longitude);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Result);
            
            return new(
                location.UserId,
                location.DisplayName,
                location.Longitude,
                location.Latitude,
                timeZone.BaseUtcOffset,
                $"{(timeZone.BaseUtcOffset < TimeSpan.Zero ? "-" : "+")}{timeZone.BaseUtcOffset:hh\\:mm}",
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
                UtcOffset: null,
                DisplayOffset: unknown,
                CountryName: unknown);
        }
    }
}