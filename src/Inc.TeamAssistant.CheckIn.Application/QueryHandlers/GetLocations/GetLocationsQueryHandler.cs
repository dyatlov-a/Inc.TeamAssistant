using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations.Convertors;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetLocations;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetLocations;

internal sealed class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, GetLocationsResult>
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly LocationConvertor _locationConvertor;

    public GetLocationsQueryHandler(ILocationsRepository locationsRepository, LocationConvertor locationConvertor)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
        _locationConvertor = locationConvertor ?? throw new ArgumentNullException(nameof(locationConvertor));
    }

    public async Task<GetLocationsResult> Handle(GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var locations = await _locationsRepository.GetLocations(query.MapId, cancellationToken);

        return new(locations
            .GroupBy(l => l.DisplayName)
            .ToDictionary(l => l.Key, l => (IReadOnlyCollection<LocationDto>)l
                .OrderByDescending(i => i.Created)
                .Select(_locationConvertor.ConvertFrom)
                .ToArray()));
    }
}