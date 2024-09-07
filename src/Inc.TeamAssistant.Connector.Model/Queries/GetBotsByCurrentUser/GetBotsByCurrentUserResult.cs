namespace Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;

public sealed record GetBotsByCurrentUserResult(IReadOnlyCollection<BotDto> Bots);