using Inc.TeamAssistant.RandomCoffee.Application.Contracts;
using Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;
using MediatR;

namespace Inc.TeamAssistant.RandomCoffee.Application.QueryHandlers.GetChats;

internal sealed class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, GetChatsResult>
{
    private readonly IRandomCoffeeReader _reader;

    public GetChatsQueryHandler(IRandomCoffeeReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetChatsResult> Handle(GetChatsQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var chats = await _reader.GetChats(query.BotId, token);

        return new GetChatsResult(chats);
    }
}