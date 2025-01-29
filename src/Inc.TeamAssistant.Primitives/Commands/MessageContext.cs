using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;

namespace Inc.TeamAssistant.Primitives.Commands;

public sealed class MessageContext
{
    private const string DefaultCommand = "/";
    
    public ChatMessage ChatMessage { get; }
    public BotContext Bot { get; }
    public IReadOnlyList<TeamContext> Teams { get; }
    public string Text { private set; get; }
    public Person Person { get; }
    public LanguageId LanguageId { get; }
    public Point? Location { get; }
    public long? TargetPersonId { get; }
    public string? ChatName { get; }

    private MessageContext(
        ChatMessage chatMessage,
        BotContext bot,
        IReadOnlyList<TeamContext> teams,
        string text,
        Person person,
        LanguageId languageId,
        Point? location,
        long? targetPersonId,
        string? chatName)
    {
        ChatMessage = chatMessage ?? throw new ArgumentNullException(nameof(chatMessage));
        Bot = bot ?? throw new ArgumentNullException(nameof(bot));
        Teams = teams ?? throw new ArgumentNullException(nameof(teams));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Person = person ?? throw new ArgumentNullException(nameof(person));
        LanguageId = languageId ?? throw new ArgumentNullException(nameof(languageId));
        Location = location;
        TargetPersonId = targetPersonId;
        ChatName = chatName;
    }

    public TargetChat TargetChat => new(Person.Id, ChatMessage.ChatId);
    
    public TeamContext? FindTeam(Guid teamId) => Teams.SingleOrDefault(t => t.Id == teamId);
    
    public Guid TryParseId(string command = DefaultCommand)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command);

        var parameters = Text.Replace(command, string.Empty, StringComparison.InvariantCultureIgnoreCase);
        return Guid.TryParse(parameters, out var value) ? value : Guid.Empty;
    }

    public MessageContext ChangeText(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Text = value;

        return this;
    }

    public static MessageContext Create(
        ChatMessage chatMessage,
        BotContext bot,
        IReadOnlyList<TeamContext> teams,
        string text,
        Person person,
        LanguageId languageId,
        Point? location,
        long? targetPersonId,
        string? chatName)
    {
        ArgumentNullException.ThrowIfNull(chatMessage);
        ArgumentNullException.ThrowIfNull(bot);
        ArgumentNullException.ThrowIfNull(teams);
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(person);
        ArgumentNullException.ThrowIfNull(languageId);
        
        return new MessageContext(
            chatMessage,
            bot,
            teams,
            text,
            person,
            languageId,
            location,
            targetPersonId,
            chatName);
    }

    public static MessageContext CreateFromBackground(Guid botId, long chatId)
    {
        return new MessageContext(
            new ChatMessage(chatId, MessageId: 0),
            new BotContext(botId, UserName: string.Empty, new Dictionary<string, string>()),
            teams: Array.Empty<TeamContext>(),
            text: string.Empty,
            Person.Empty,
            LanguageSettings.DefaultLanguageId,
            location: null,
            targetPersonId: null,
            chatName: string.Empty);
    }

    public static MessageContext CreateFromIntegration(Guid botId, Guid teamId, long chatId, long personId)
    {
        return new MessageContext(
            ChatMessage.Empty,
            new BotContext(botId, UserName: string.Empty, new Dictionary<string, string>()),
            [new TeamContext(teamId, chatId, Name: string.Empty, UserInTeam: false, OwnerOfTeam: false)],
            text: string.Empty,
            new Person(personId, string.Empty, Username: null),
            LanguageSettings.DefaultLanguageId,
            location: null,
            targetPersonId: null,
            chatName: null);
    }
}