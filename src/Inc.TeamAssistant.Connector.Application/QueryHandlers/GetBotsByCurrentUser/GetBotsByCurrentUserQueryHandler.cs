using Inc.TeamAssistant.Connector.Application.Contracts;
using Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetBotsByCurrentUser;

internal sealed class GetBotsByCurrentUserQueryHandler
    : IRequestHandler<GetBotsByCurrentUserQuery, GetBotsByCurrentUserResult>
{
    private readonly IBotReader _reader;
    private readonly IPersonResolver _personResolver;

    public GetBotsByCurrentUserQueryHandler(IBotReader reader, IPersonResolver personResolver)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task<GetBotsByCurrentUserResult> Handle(GetBotsByCurrentUserQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var person = _personResolver.GetCurrentPerson();
        var bots = await _reader.GetBotsByUser(person.Id, token);

        return new GetBotsByCurrentUserResult(bots);
    }
}