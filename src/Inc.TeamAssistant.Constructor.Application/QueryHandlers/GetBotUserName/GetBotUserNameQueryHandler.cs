using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBotUserName;

internal sealed class GetBotUserNameQueryHandler : IRequestHandler<GetBotUserNameQuery, GetBotUserNameResult>
{
    private readonly IBotConnector _botConnector;

    public GetBotUserNameQueryHandler(IBotConnector botConnector)
    {
        _botConnector = botConnector ?? throw new ArgumentNullException(nameof(botConnector));
    }

    public async Task<GetBotUserNameResult> Handle(GetBotUserNameQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var userName = await _botConnector.CheckAccess(query.Token, token);

        return new GetBotUserNameResult(HasAccess: !string.IsNullOrWhiteSpace(userName), userName);
    }
}