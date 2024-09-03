namespace Inc.TeamAssistant.Connector.Model.Queries.GetBots;

public sealed record BotDto(
    Guid Id,
    string Name,
    long OwnerId,
    IReadOnlyCollection<string> Features,
    IReadOnlyCollection<TeamDto> Teams);