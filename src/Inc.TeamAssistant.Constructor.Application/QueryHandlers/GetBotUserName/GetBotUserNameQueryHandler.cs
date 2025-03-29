using Inc.TeamAssistant.Constructor.Model.Queries.GetBotUserName;
using Inc.TeamAssistant.Primitives.Bots;
using MediatR;

namespace Inc.TeamAssistant.Constructor.Application.QueryHandlers.GetBotUserName;

internal sealed class GetBotUserNameQueryHandler : IRequestHandler<GetBotUserNameQuery, GetBotUserNameResult>
{
    private readonly IBotConnector _connector;

    public GetBotUserNameQueryHandler(IBotConnector connector)
    {
        _connector = connector ?? throw new ArgumentNullException(nameof(connector));
    }

    public async Task<GetBotUserNameResult> Handle(GetBotUserNameQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);
        
        var userName = await _connector.GetUsername(query.Token, token);
        
        var hasAccess = !string.IsNullOrWhiteSpace(userName);
        return new GetBotUserNameResult(hasAccess, userName ?? string.Empty);
    }
}