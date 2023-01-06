namespace Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;

public sealed class DialogState
{
    private readonly List<int> _messageIds;
    private readonly List<string> _data;

    public DialogState(string continuationState)
    {
        if (string.IsNullOrWhiteSpace(continuationState))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(continuationState));

        ContinuationState = continuationState;
        _messageIds = new();
        _data = new();
    }

    public string ContinuationState { get; }

    public IReadOnlyCollection<int> MessageIds => _messageIds;
    public IReadOnlyCollection<string> Data => _data;

    public DialogState AttachMessage(int messageId)
    {
        _messageIds.Add(messageId);
        return this;
    }

    public void AddToData(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(item));

        _data.Add(item);
    }
}