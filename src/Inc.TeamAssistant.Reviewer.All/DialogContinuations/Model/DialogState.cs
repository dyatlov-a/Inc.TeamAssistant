namespace Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;

public sealed class DialogState
{
    private readonly List<ChatMessage> _chatMessages = new();
    private readonly List<string> _data = new();
    public string ContinuationState { get; }

    public DialogState(string continuationState)
    {
        if (string.IsNullOrWhiteSpace(continuationState))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(continuationState));

        ContinuationState = continuationState;
    }

    public IReadOnlyCollection<ChatMessage> ChatMessages => _chatMessages;
    public IReadOnlyCollection<string> Data => _data;

    public DialogState TryAttachMessage(ChatMessage? chatMessage)
    {
        if (chatMessage is { })
            _chatMessages.Add(chatMessage);
        
        return this;
    }

    public DialogState AddItem(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(item));

        _data.Add(item);
        
        return this;
    }
}