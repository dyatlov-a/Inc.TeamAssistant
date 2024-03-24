namespace Inc.TeamAssistant.Primitives.Commands;

public sealed record TeamContext(
    Guid Id,
    long ChatId,
    string Name,
    bool UserInTeam);