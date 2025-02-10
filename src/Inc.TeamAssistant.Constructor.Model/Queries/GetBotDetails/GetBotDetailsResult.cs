using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;

public sealed record GetBotDetailsResult(IReadOnlyCollection<BotDetails> Items)
    : IWithEmpty<GetBotDetailsResult>
{
    public static GetBotDetailsResult Empty { get; } = new(Array.Empty<BotDetails>());
}