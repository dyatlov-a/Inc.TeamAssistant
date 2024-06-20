using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;
using MediatR;

namespace Inc.TeamAssistant.CheckIn.Application.QueryHandlers.GetMaps;

internal sealed class GetMapsQueryHandler : IRequestHandler<GetMapsQuery, GetMapsResult>
{
    private readonly ILocationsRepository _locationsRepository;

    public GetMapsQueryHandler(ILocationsRepository locationsRepository)
    {
        _locationsRepository = locationsRepository ?? throw new ArgumentNullException(nameof(locationsRepository));
    }

    public async Task<GetMapsResult> Handle(GetMapsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var maps = await _locationsRepository.GetByBot(query.BotId, token);

        var results = maps
            .Select(m => new MapDto(m.Id, m.Name ?? m.Id.ToString()))
            .ToArray();

        return new GetMapsResult(results);
    }
}