using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Primitives.Notifications;

public sealed class NotificationMessage
{
    private readonly List<Button> _buttons = new();
    private readonly List<string> _options = new();
    private readonly List<(Person Person, LanguageId LanguageId, int Offset)> _targetPersons = new();
    
	public delegate IContinuationCommand ResponseHandler(
        MessageContext messageContext,
        int messageId,
        string? pollId);
    
	public string Text { get; }
    public bool Pinned { get; }
    public long? ChatId { get; }
    public ChatMessage? EditedMessage { get; }
    public ChatMessage? DeletedMessage { get; }
    public int? PollMessageId { get; }
    public IReadOnlyCollection<Button> Buttons => _buttons;
    public IReadOnlyCollection<string> Options => _options;
    public int ButtonsInRow { get; private set; }
    public ResponseHandler? Handler { get; private set; }
    public IReadOnlyCollection<(Person Person, LanguageId LanguageId, int Offset)> TargetPersons => _targetPersons;
    public int? ReplyToMessageId { get; private set; }

    private NotificationMessage(
        long? chatId,
        ChatMessage? editedMessage,
        ChatMessage? deletedMessage,
        string text,
        bool pinned = false,
        int? pollMessageId = null)
    {
        const int defaultButtonsInRow = 5;
        
        Text = text ?? throw new ArgumentNullException(nameof(text));
        ChatId = chatId;
        EditedMessage = editedMessage;
        DeletedMessage = deletedMessage;
        Pinned = pinned;
        ButtonsInRow = defaultButtonsInRow;
        PollMessageId = pollMessageId;
    }

    public NotificationMessage WithHandler(ResponseHandler handler)
    {
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));

        return this;
    }

    public NotificationMessage SetButtonsInRow(int value)
    {
        ButtonsInRow = value;

        return this;
    }

    public NotificationMessage WithButton(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        _buttons.Add(button);

        return this;
    }
    
    public NotificationMessage WithOption(string option)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(option);
        
        _options.Add(option);

        return this;
    }

    public NotificationMessage AttachPerson(Person person, LanguageId languageId, int offset)
    {
        ArgumentNullException.ThrowIfNull(person);
        ArgumentNullException.ThrowIfNull(languageId);

        _targetPersons.Add((person, languageId, offset));

        return this;
    }
    
    public NotificationMessage ReplyTo(int messageId)
    {
        ReplyToMessageId = messageId;

        return this;
    }

    public static NotificationMessage Create(long targetChatId, string text, bool pinned = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        return new(
            targetChatId,
            editedMessage: null,
            deletedMessage: null,
            text,
            pinned);
    }

    public static NotificationMessage Edit(ChatMessage targetMessage, string text)
    {
        ArgumentNullException.ThrowIfNull(targetMessage);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        return new(
            chatId: null,
            targetMessage,
            deletedMessage: null,
            text);
    }

    public static NotificationMessage Delete(ChatMessage deleteMessage)
    {
        ArgumentNullException.ThrowIfNull(deleteMessage);

        return new(
            chatId: null,
            editedMessage: null,
            deleteMessage,
            text: string.Empty);
    }

    public static NotificationMessage EndPoll(long chatId, int pollId)
    {
        return new(
            chatId,
            editedMessage: null,
            deletedMessage: null,
            text: string.Empty,
            pollMessageId: pollId);
    }
    
    public NotificationMessage AddIf(bool condition, Action<NotificationMessage> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        return AddIfElse(condition, action, _ => {});
    }
    
    public NotificationMessage AddIfElse(
        bool condition,
        Action<NotificationMessage> firstAction,
        Action<NotificationMessage> secondAction)
    {
        ArgumentNullException.ThrowIfNull(firstAction);
        ArgumentNullException.ThrowIfNull(secondAction);

        if (condition)
            firstAction(this);
        else
            secondAction(this);

        return this;
    }
}