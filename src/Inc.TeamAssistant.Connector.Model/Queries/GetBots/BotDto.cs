namespace Inc.TeamAssistant.Connector.Model.Queries.GetBots;

public sealed record BotDto(
    Guid Id,
    string Name,
    IReadOnlyCollection<string> Features,
    IReadOnlyCollection<TeamDto> Teams);