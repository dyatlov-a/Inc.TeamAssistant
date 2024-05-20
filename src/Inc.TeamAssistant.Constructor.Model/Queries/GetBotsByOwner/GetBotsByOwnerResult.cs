namespace Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;

public sealed record GetBotsByOwnerResult(IReadOnlyCollection<BotDto> Bots);