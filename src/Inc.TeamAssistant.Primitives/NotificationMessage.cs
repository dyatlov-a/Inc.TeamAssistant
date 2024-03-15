namespace Inc.TeamAssistant.Primitives;

public sealed class NotificationMessage
{
    private readonly List<Button> _buttons = new();
    
	public delegate IContinuationCommand ResponseHandler(MessageContext messageContext, int messageId);
    
	public string Text { get; }
    public bool Pinned { get; }
    public long? TargetChatId { get; }
    public ChatMessage? TargetMessage { get; }
    public ChatMessage? DeleteMessage { get; }
    public IReadOnlyCollection<Button> Buttons => _buttons;
    public int ButtonsInRow { get; private set; }
    public ResponseHandler? Handler { get; private set; }
    public long? TargetPersonId { get; private set; }

    private NotificationMessage(
        long? targetChatId,
        ChatMessage? targetMessage,
        ChatMessage? deleteMessage,
        string text,
        bool pinned = false)
    {
        const int defaultButtonsInRow = 5;
        
        Text = text ?? throw new ArgumentNullException(nameof(text));
        TargetChatId = targetChatId;
        TargetMessage = targetMessage;
        DeleteMessage = deleteMessage;
        Pinned = pinned;
        ButtonsInRow = defaultButtonsInRow;
    }

    public NotificationMessage AddHandler(ResponseHandler handler)
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
        if (button is null)
            throw new ArgumentNullException(nameof(button));
        
        _buttons.Add(button);

        return this;
    }

    public NotificationMessage AttachPerson(long? targetPersonId)
    {
        TargetPersonId = targetPersonId;

        return this;
    }

    public static NotificationMessage Create(long targetChatId, string text, bool pinned = false)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        return new(targetChatId, targetMessage: null, deleteMessage: null, text, pinned);
    }

    public static NotificationMessage Edit(ChatMessage targetMessage, string text)
    {
        if (targetMessage is null)
            throw new ArgumentNullException(nameof(targetMessage));
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

        return new(targetChatId: null, targetMessage, deleteMessage: null, text);
    }

    public static NotificationMessage Delete(ChatMessage deleteMessage)
    {
        if (deleteMessage is null)
            throw new ArgumentNullException(nameof(deleteMessage));
        
        return new(targetChatId: null, targetMessage: null, deleteMessage, string.Empty);
    }
}