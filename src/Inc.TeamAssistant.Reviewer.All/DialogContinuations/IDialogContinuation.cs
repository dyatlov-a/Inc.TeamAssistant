using Inc.TeamAssistant.Reviewer.All.DialogContinuations.Model;

namespace Inc.TeamAssistant.Reviewer.All.DialogContinuations;

internal interface IDialogContinuation
{
    DialogState? Find(long userId);
    DialogState? TryBegin(long userId, string continuationState, int messageId);
    void End(long userId, string continuationState, int messageId);
}