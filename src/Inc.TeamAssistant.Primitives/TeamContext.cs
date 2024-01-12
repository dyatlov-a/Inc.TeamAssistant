namespace Inc.TeamAssistant.Primitives;

public sealed record TeamContext(Guid Id, long ChatId, string Name, bool UserInTeam);