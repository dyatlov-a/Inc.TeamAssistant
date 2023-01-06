using GeoTimeZone;
using Inc.TeamAssistant.Appraiser.Model.CheckIn;
using Inc.TeamAssistant.Appraiser.Model.CheckIn.Queries.GetLocations;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.CheckIn.All.Contracts;
using Inc.TeamAssistant.CheckIn.All.Model;

namespace Inc.TeamAssistant.Appraiser.Backend.Services.CheckIn;

internal sealed class CheckInService : ICheckInService
{
    private readonly ILocationsRepository _locationsRepository;

    public CheckInService(ILocationsRepository locationsRepository)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
    }

    public async Task<ServiceResult<GetLocationsResult?>> GetLocations(Guid mapId, CancellationToken cancellationToken)
    {
        try
        {
            var locations = await _locationsRepository.GetLocations(mapId, cancellationToken);

            return locations.Any()
                ? ServiceResult.Success((GetLocationsResult?)new(locations
                    .GroupBy(l => l.DisplayName)
                    .ToDictionary(l => l.Key, l => (IReadOnlyCollection<LocationDto>)l
                        .OrderByDescending(i => i.Created)
                        .Select(ConvertFrom)
                        .ToArray())))
                : ServiceResult.NotFound<GetLocationsResult?>();
        }
        catch (Exception ex)
        {
            return ServiceResult.Failed<GetLocationsResult?>(ex.Message);
        }
    }

    private LocationDto ConvertFrom(LocationOnMap location)
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));

        var timeZoneId = TimeZoneLookup.GetTimeZone(location.Latitude, location.Longitude).Result;
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        return new(location.DisplayName, location.Longitude, location.Latitude, timeZone.BaseUtcOffset);
    }
}