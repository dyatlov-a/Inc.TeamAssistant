using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetBotsByCurrentUser;

internal sealed class GetBotsByCurrentUserQueryHandler
    : IRequestHandler<GetBotsByCurrentUserQuery, GetBotsByCurrentUserResult>
{
    private readonly IBotReader _botReader;
    private readonly ICurrentPersonResolver _currentPersonResolver;

    public GetBotsByCurrentUserQueryHandler(IBotReader botReader, ICurrentPersonResolver currentPersonResolver)
    {
        _botReader = botReader ?? throw new ArgumentNullException(nameof(botReader));
        _currentPersonResolver
            = currentPersonResolver ?? throw new ArgumentNullException(nameof(currentPersonResolver));
    }

    public async Task<GetBotsByCurrentUserResult> Handle(GetBotsByCurrentUserQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var person = _currentPersonResolver.GetCurrentPerson();
        var bots = await _botReader.GetBotsByUser(person.Id, token);

        return new GetBotsByCurrentUserResult(bots);
    }
}