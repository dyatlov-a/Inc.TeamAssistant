using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetBots;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetBots;

internal sealed class GetBotsQueryHandler : IRequestHandler<GetBotsQuery, GetBotsResult>
{
    private readonly IBotReader _botReader;

    public GetBotsQueryHandler(IBotReader botReader)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
    }

    public async Task<GetBotsResult> Handle(GetBotsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var bots = await _botReader.GetBotsByUser(query.UserId, token);
        var items = bots
            .Select(b => new BotDto(
                b.Id,
                b.Name,
                b.Teams.Select(t => new TeamDto(t.Id, t.Name)).ToArray()))
            .ToArray();

        return new GetBotsResult(items);
    }
}