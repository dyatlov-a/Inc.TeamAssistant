using System.Collections.Concurrent;
using Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;

namespace Inc.TeamAssistant.Reviewer.All.DialogContinuations.Internal;

internal sealed class DialogContinuation : IDialogContinuation
{
    private readonly ConcurrentDictionary<long, DialogState> _store = new();

    public DialogState? Find(long userId) => _store.TryGetValue(userId, out var value) ? value : null;

    public DialogState Begin(long userId, string continuationState, int messageId)
    {
        if (string.IsNullOrWhiteSpace(continuationState))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(continuationState));

        var dialogState = new DialogState(continuationState);
        dialogState.AttachMessage(messageId);
        _store.AddOrUpdate(userId, u => dialogState, (u, s) => dialogState);
        return dialogState;
    }

    public void End(long userId, string continuationState, int messageId)
    {
        if (string.IsNullOrWhiteSpace(continuationState))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(continuationState));

        if (!_store.TryGetValue(userId, out var value))
            throw new ApplicationException($"Operation ({userId}, {continuationState}) is not began.");

        if (!value.ContinuationState.Equals(continuationState, StringComparison.InvariantCultureIgnoreCase))
            throw new ApplicationException($"Trying ({userId}, {continuationState}) End other operation.");

        value.AttachMessage(messageId);
        _store.Remove(userId, out _);
    }
}