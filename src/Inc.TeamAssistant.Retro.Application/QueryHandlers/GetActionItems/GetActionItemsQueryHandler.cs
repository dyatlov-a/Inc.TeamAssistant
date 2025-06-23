using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
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

        var firstPage = (Guid?)null;
        var properties = await _provider.Get(query.RoomId, token);
        var newItems = await _reader.Read(query.RoomId, ActionItemState.New, firstPage, pageSize: int.MaxValue, token);
        var doneItems = await _reader.Read(query.RoomId, ActionItemState.Done, firstPage, query.PageSize, token);
        var pinnedItems = await _reader.Read(query.RoomId, ActionItemState.Pinned, firstPage, query.PageSize, token);
        var actionItems = newItems
            .Union(doneItems)
            .Union(pinnedItems)
            .Select(ActionItemConverter.ConvertTo)
            .ToArray();
        
        return new GetActionItemsResult(properties.FacilitatorId, actionItems);
    }
}