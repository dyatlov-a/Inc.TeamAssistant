using Inc.TeamAssistant.Primitives.Bots;

namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBotDetails;

public sealed record GetBotDetailsResult(IReadOnlyCollection<BotDetails> Items);