using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;

public sealed record GetBotsByCurrentUserResult(IReadOnlyCollection<BotDto> Bots)
    : IWithEmpty<GetBotsByCurrentUserResult>
{
    public static GetBotsByCurrentUserResult Empty { get; } = new(Array.Empty<BotDto>());
}