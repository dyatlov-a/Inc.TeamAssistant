namespace Inc.TeamAssistant.Primitives.Bots;

public sealed record BotDetails(
    string LanguageId,
    string Name,
    string ShortDescription,
    string Description);