namespace Inc.TeamAssistant.Primitives;

public sealed record CurrentTeamContext(Guid TeamId, IReadOnlyDictionary<string, string> Properties);