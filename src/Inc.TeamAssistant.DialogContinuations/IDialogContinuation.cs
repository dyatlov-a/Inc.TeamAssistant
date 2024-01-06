using Inc.TeamAssistant.DialogContinuations.Model;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.DialogContinuations;

public interface IDialogContinuation<T>
    where T : notnull
{
    DialogState<T>? Find(long userId);
    DialogState<T>? TryBegin(long userId, T continuationState, ChatMessage? chatMessage = null);
    DialogState<T>? TryEnd(long userId, T continuationState, ChatMessage? chatMessage = null);
}