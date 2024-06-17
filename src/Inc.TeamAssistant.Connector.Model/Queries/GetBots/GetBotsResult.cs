namespace Inc.TeamAssistant.Connector.Model.Queries.GetBots;

public sealed record GetBotsResult(IReadOnlyCollection<BotDto> Bots);