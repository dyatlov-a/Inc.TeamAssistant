using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetRetroItems;

internal sealed class GetRetroItemsQueryHandler : IRequestHandler<GetRetroItemsQuery, GetRetroItemsResult>
{
    private readonly IRetroReader _retroReader;

    public GetRetroItemsQueryHandler(IRetroReader retroReader)
    {
        _retroReader = retroReader ?? throw new ArgumentNullException(nameof(retroReader));
    }

    public async Task<GetRetroItemsResult> Handle(GetRetroItemsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var items = await _retroReader.GetAll(query.TeamId, token);
        var results = items
            .Select(i => new RetroItemDto(i.Id, i.TeamId, i.Created, i.Type, i.Text, i.OwnerId))
            .ToArray();

        return new GetRetroItemsResult(results);
    }
}