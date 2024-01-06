using MediatR;

namespace Inc.TeamAssistant.Primitives;

public sealed class NotificationMessage
{
    private readonly List<Button> _buttons = new();
    
	public delegate IRequest<CommandResult> ResponseHandler(long chatId, string userName, int messageId);

	public string Text { get; }
    public bool Pinned { get; }
    public IReadOnlyCollection<long>? TargetChatIds { get; }
    public IReadOnlyCollection<ChatMessage>? TargetMessages { get; }
    public IReadOnlyCollection<ChatMessage>? DeleteMessages { get; }
    public IReadOnlyCollection<Button> Buttons => _buttons;
    public ResponseHandler? Handler { get; private set; }

    private NotificationMessage(
        IReadOnlyCollection<long>? targetChatIds,
        IReadOnlyCollection<ChatMessage>? targetMessages,
        IReadOnlyCollection<ChatMessage>? deleteMessages,
        string text,
        bool pinned = false)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        TargetChatIds = targetChatIds;
        TargetMessages = targetMessages;
        DeleteMessages = deleteMessages;
        Pinned = pinned;
    }

    public NotificationMessage AddHandler(ResponseHandler handler)
    {
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));

        return this;
    }

    public NotificationMessage WithButton(Button button)
    {
        if (button is null)
            throw new ArgumentNullException(nameof(button));
        
        _buttons.Add(button);

        return this;
    }

    public static NotificationMessage Create(IReadOnlyCollection<long> targetChatIds, string text, bool pinned = false)
    {
        if (targetChatIds is null)
            throw new ArgumentNullException(nameof(targetChatIds));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        return new(targetChatIds, targetMessages: null, deleteMessages: null, text, pinned);
    }

    public static NotificationMessage Create(long targetChatId, string text, bool pinned = false)
        => Create(new[] { targetChatId }, text, pinned);

    public static NotificationMessage Edit(
        IReadOnlyCollection<ChatMessage> targetMessages,
        string text)
    {
        if (targetMessages is null)
            throw new ArgumentNullException(nameof(targetMessages));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        return new(targetChatIds: null, targetMessages, deleteMessages: null, text);
    }

    public static NotificationMessage Delete(IReadOnlyCollection<ChatMessage> deleteMessages)
    {
        if (deleteMessages is null)
            throw new ArgumentNullException(nameof(deleteMessages));
        
        return new(targetChatIds: null, targetMessages: null, deleteMessages, string.Empty);
    }
}