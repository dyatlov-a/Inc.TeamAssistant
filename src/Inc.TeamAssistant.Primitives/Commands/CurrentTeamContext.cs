namespace Inc.TeamAssistant.Primitives.Commands;

public sealed record CurrentTeamContext(Guid TeamId, IReadOnlyDictionary<string, string> Properties)
{
    public static readonly CurrentTeamContext Empty = new(Guid.Empty, new Dictionary<string, string>());
}