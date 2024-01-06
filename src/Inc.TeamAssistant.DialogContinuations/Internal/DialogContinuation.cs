using System.Collections.Concurrent;
using Inc.TeamAssistant.DialogContinuations.Model;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.DialogContinuations.Internal;

internal sealed class DialogContinuation<T> : IDialogContinuation<T>
    where T : notnull
{
    private readonly ConcurrentDictionary<long, DialogState<T>> _store = new();

    public DialogState<T>? Find(long userId) => _store.GetValueOrDefault(userId);

    public DialogState<T>? TryBegin(long userId, T continuationState, ChatMessage? chatMessage = null)
    {
        if (continuationState is null)
            throw new ArgumentNullException(nameof(continuationState));
        
        var dialogState = new DialogState<T>(continuationState).TryAttachMessage(chatMessage);

        if (_store.TryAdd(userId, dialogState))
            return dialogState;

        return null;
    }

    public DialogState<T>? TryEnd(long userId, T continuationState, ChatMessage? chatMessage = null)
    {
        if (continuationState is null)
            throw new ArgumentNullException(nameof(continuationState));
        if (!_store.TryGetValue(userId, out var value))
            return null;
        if (!value.ContinuationState.Equals(continuationState))
            throw new ApplicationException($"Trying ({userId}, {continuationState}) End other operation.");
        
        value.TryAttachMessage(chatMessage);
        
        _store.Remove(userId, out _);

        return value;
    }
}