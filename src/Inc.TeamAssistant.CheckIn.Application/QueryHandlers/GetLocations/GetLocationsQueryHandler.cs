using GeoTimeZone;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Domain;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations;

internal sealed class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsResult>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly ILogger<GetLocationsQueryHandler> _logger;

    public GetLocationsQueryHandler(ILocationsRepository locationsRepository, ILogger<GetLocationsQueryHandler> logger)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetLocationsResult> Handle(GetLocationsQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));
        
        var locations = await _locationsRepository.GetLocations(query.MapId, cancellationToken);
        var results = locations
            .GroupBy(l => l.DisplayName)
            .ToDictionary(l => l.Key, l => (IReadOnlyCollection<LocationDto>)l
                .OrderByDescending(i => i.Created)
                .Select(ConvertFrom)
                .ToArray());

        return new(results);
    }
    
    private LocationDto ConvertFrom(LocationOnMap location)
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