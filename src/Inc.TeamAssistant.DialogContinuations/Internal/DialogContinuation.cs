using System.Collections.Concurrent;
using Inc.TeamAssistant.DialogContinuations.Model;

namespace Inc.TeamAssistant.DialogContinuations.Internal;

internal sealed class DialogContinuation<T> : IDialogContinuation<T>
    where T : notnull
{
    private readonly ConcurrentDictionary<long, DialogState<T>> _store = new();

    public DialogState<T>? Find(long userId)
    {
        return _store.TryGetValue(userId, out var value) ? value : null;
    }

    public DialogState<T>? TryBegin(long userId, T continuationState, ChatMessage? chatMessage = null)
    {
        if (continuationState is null)
            throw new ArgumentNullException(nameof(continuationState));
        
        var dialogState = new DialogState<T>(continuationState).TryAttachMessage(chatMessage);

        if (_store.TryAdd(userId, dialogState))
            return dialogState;

        return null;
    }

    public void End(long userId, T continuationState, ChatMessage? chatMessage = null)
    {
        if (continuationState is null)
            throw new ArgumentNullException(nameof(continuationState));
        if (!_store.TryGetValue(userId, out var value))
            throw new ApplicationException($"Operation ({userId}, {continuationState}) is not began.");
        if (!value.ContinuationState.Equals(continuationState))
            throw new ApplicationException($"Trying ({userId}, {continuationState}) End other operation.");
        
        value.TryAttachMessage(chatMessage);
        
        _store.Remove(userId, out _);
    }
}