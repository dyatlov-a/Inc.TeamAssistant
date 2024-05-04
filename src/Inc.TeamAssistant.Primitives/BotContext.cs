namespace Inc.TeamAssistant.Primitives;

public sealed record BotContext(Guid Id, string UserName)
{
    public static readonly BotContext Empty = new(Guid.Empty, string.Empty);
}