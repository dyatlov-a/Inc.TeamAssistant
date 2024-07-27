using Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBotDetails;

internal sealed class GetBotDetailsQueryHandler : IRequestHandler<GetBotDetailsQuery, GetBotDetailsResult>
{
    private readonly IBotConnector _botConnector;

    public GetBotDetailsQueryHandler(IBotConnector botConnector)
    {
        _botConnector = botConnector ?? throw new ArgumentNullException(nameof(botConnector));
    }

    public async Task<GetBotDetailsResult> Handle(GetBotDetailsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var details = await _botConnector.GetDetails(query.Token, token);

        return new GetBotDetailsResult(details);
    }
}