using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetActionItems;

internal sealed class GetActionItemsQueryHandler : IRequestHandler<GetActionItemsQuery, GetActionItemsResult>
{
    private readonly IActionItemReader _reader;
    private readonly IRetroPropertiesProvider _provider;

    public GetActionItemsQueryHandler(IActionItemReader reader, IRetroPropertiesProvider provider)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task<GetActionItemsResult> Handle(GetActionItemsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var items = await _reader.Read(query.RoomId, token);
        var actionItems = items
            .Select(ActionItemConverter.ConvertTo)
            .ToArray();
        var properties = await _provider.Get(query.RoomId, token);
        
        return new GetActionItemsResult(properties.FacilitatorId, actionItems);
    }
}