namespace Inc.TeamAssistant.Primitives.Bots;

public sealed record BotContext(Guid Id, string UserName)
{
    public static readonly BotContext Empty = new(Guid.Empty, string.Empty);
}