using Inc.TeamAssistant.DialogContinuations.Model;

namespace Inc.TeamAssistant.DialogContinuations;

public interface IDialogContinuation<T>
    where T : notnull
{
    DialogState<T>? Find(long userId);
    DialogState<T>? TryBegin(long userId, T continuationState, ChatMessage? chatMessage = null);
    void End(long userId, T continuationState, ChatMessage? chatMessage = null);
}