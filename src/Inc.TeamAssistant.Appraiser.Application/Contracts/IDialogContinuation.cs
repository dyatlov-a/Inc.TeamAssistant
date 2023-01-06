using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IDialogContinuation
{
    ContinuationState? Find(ParticipantId participantId);

    void Begin(ParticipantId participantId, ContinuationState continuationState);

    void End(ParticipantId participantId, ContinuationState continuationState);
}