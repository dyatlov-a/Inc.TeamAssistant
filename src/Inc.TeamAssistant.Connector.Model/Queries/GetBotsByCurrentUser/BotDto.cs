namespace Inc.TeamAssistant.Connector.Model.Queries.GetBotsByCurrentUser;

public sealed record BotDto(
    Guid Id,
    string Name,
    long OwnerId,
    IReadOnlyCollection<TeamDto> Teams);