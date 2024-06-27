using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetBots;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetBots;

internal sealed class GetBotsQueryHandler : IRequestHandler<GetBotsQuery, GetBotsResult>
{
    private readonly IBotReader _botReader;
    private readonly ILinkBuilder _linkBuilder;

    public GetBotsQueryHandler(IBotReader botReader, ILinkBuilder linkBuilder)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
    }

    public async Task<GetBotsResult> Handle(GetBotsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var bots = await _botReader.GetBotsByUser(query.UserId, _linkBuilder.BuildLinkForConnect, token);

        return new GetBotsResult(bots);
    }
}