using Inc.TeamAssistant.Retro.Application.Common.Converters;
using Inc.TeamAssistant.Retro.Application.Contracts;
using Inc.TeamAssistant.Retro.Model.Queries.GetActionItems;
using MediatR;

namespace Inc.TeamAssistant.Retro.Application.QueryHandlers.GetActionItems;

internal sealed class GetActionItemsQueryHandler : IRequestHandler<GetActionItemsQuery, GetActionItemsResult>
{
    private readonly IActionItemReader _reader;

    public GetActionItemsQueryHandler(IActionItemReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetActionItemsResult> Handle(GetActionItemsQuery query, CancellationToken token)
    {
        var items = await _reader.Read(query.TeamId, token);
        var actionItems = items
            .Select(ActionItemConverter.ConvertTo)
            .ToArray();
        
        return new GetActionItemsResult(actionItems);
    }
}