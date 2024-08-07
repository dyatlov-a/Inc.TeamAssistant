using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Primitives.Commands;

public sealed record MessageContext(
    ChatMessage ChatMessage,
    BotContext Bot,
    IReadOnlyList<TeamContext> Teams,
    string Text,
    Person Person,
    LanguageId LanguageId,
    Point? Location,
    long? TargetPersonId,
    string? ChatName)
{
    public TargetChat TargetChat => new(Person.Id, ChatMessage.ChatId);

    public static MessageContext CreateIdle(Guid botId, long chatId)
    {
        return new MessageContext(
            ChatMessage: new ChatMessage(chatId, MessageId: 0),
            Bot: new BotContext(botId, UserName: string.Empty, new Dictionary<string, string>()),
            Teams: Array.Empty<TeamContext>(),
            Text: string.Empty,
            Person.Empty,
            LanguageId: LanguageSettings.DefaultLanguageId,
            Location: null,
            TargetPersonId: null,
            ChatName: null);
    }
        
    public TeamContext? FindTeam(Guid teamId) => Teams.SingleOrDefault(t => t.Id == teamId);

    public Guid TryParseId(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));

        var parameters = Text.Replace(command, string.Empty, StringComparison.InvariantCultureIgnoreCase);
        return Guid.TryParse(parameters, out var value) ? value : Guid.Empty;
    }
}