using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItemsHistory;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetActionItemsHistory;

internal sealed class GetActionItemsHistoryQueryHandler
    : IRequestHandler<GetActionItemsHistoryQuery, GetActionItemsHistoryResult>
{
    private readonly IActionItemReader _reader;

    public GetActionItemsHistoryQueryHandler(IActionItemReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetActionItemsHistoryResult> Handle(GetActionItemsHistoryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var state = Enum.Parse<ActionItemState>(query.State, ignoreCase: true);
        var items = await _reader.Read(query.RoomId, state, query.Offset, query.Limit, token);
        var actionItems = items
            .Select(ActionItemConverter.ConvertTo)
            .ToArray();
        
        return new GetActionItemsHistoryResult(actionItems);
    }
}