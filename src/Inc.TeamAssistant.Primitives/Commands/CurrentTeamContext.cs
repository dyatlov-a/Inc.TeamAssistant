namespace Inc.TeamAssistant.Primitives.Commands;

public sealed record CurrentTeamContext(
    Guid TeamId,
    string Name,
    IReadOnlyDictionary<string, string> Properties,
    Guid BotId)
{
    public static readonly CurrentTeamContext Empty = new(
        Guid.Empty,
        string.Empty,
        new Dictionary<string, string>(),
        Guid.Empty);
}