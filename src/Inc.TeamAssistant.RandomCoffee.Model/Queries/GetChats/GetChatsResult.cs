using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.RandomCoffee.Model.Queries.GetChats;

public sealed record GetChatsResult(IReadOnlyCollection<ChatDto> Items)
    : IWithEmpty<GetChatsResult>
{
    public static GetChatsResult Empty { get; } = new(Array.Empty<ChatDto>());
}