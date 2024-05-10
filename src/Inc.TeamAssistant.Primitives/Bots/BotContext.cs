namespace Inc.TeamAssistant.Primitives.Bots;

public sealed record BotContext(Guid Id, string UserName, IReadOnlyDictionary<string, string> Properties)
{
    public static readonly BotContext Empty = new(Guid.Empty, string.Empty, new Dictionary<string, string>());
}