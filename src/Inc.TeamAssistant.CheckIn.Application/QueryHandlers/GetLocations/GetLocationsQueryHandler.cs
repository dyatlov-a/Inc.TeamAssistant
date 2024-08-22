using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Services;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations;

internal sealed class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsResult>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly LocationConverter _locationConverter;

    public GetLocationsQueryHandler(ILocationsRepository locationsRepository, LocationConverter locationConverter)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _locationConverter = locationConverter ?? throw new ArgumentNullException(nameof(locationConverter));
    }

    public async Task<GetLocationsResult> Handle(GetLocationsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var locations = await _locationsRepository.GetLocations(query.MapId, token);
        var locationsByUser = locations
            .GroupBy(l => l.DisplayName)
            .ToDictionary(l => l.Key, l => _locationConverter.Convert(l));

        return new(locationsByUser);
    }
}