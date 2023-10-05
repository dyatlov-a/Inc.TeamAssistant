namespace Inc.TeamAssistant.DialogContinuations.Model;

public sealed class DialogState<T>
    where T : notnull
{
    private readonly List<ChatMessage> _chatMessages = new();
    private readonly List<string> _data = new();
    public T ContinuationState { get; }

    public DialogState(T continuationState)
    {
        ContinuationState = continuationState ?? throw new ArgumentNullException(nameof(continuationState));
    }

    public IReadOnlyCollection<ChatMessage> ChatMessages => _chatMessages;
    public IReadOnlyCollection<string> Data => _data;

    public DialogState<T> TryAttachMessage(ChatMessage? chatMessage)
    {
        if (chatMessage is { })
            _chatMessages.Add(chatMessage);
        
        return this;
    }

    public DialogState<T> AddItem(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(item));

        _data.Add(item);
        
        return this;
    }
}