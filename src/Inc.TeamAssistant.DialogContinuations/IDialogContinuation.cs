using Inc.TeamAssistant.DialogContinuations.Model;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.DialogContinuations;

public interface IDialogContinuation<T>
    where T : notnull
{
    DialogState<T>? Find(long userId);
    bool TryBegin(long userId, T continuationState, out DialogState<T> dialogState, ChatMessage? chatMessage = null);
    DialogState<T>? TryEnd(long userId, T continuationState, ChatMessage? chatMessage = null);
}