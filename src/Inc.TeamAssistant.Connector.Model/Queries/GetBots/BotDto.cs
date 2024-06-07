namespace Inc.TeamAssistant.Connector.Model.Queries.GetBots;

public sealed record BotDto(Guid Id, string Name, IReadOnlyCollection<TeamDto> Teams);