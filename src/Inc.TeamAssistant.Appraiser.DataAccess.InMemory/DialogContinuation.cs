using System.Collections.Concurrent;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.DataAccess.InMemory;

internal sealed class DialogContinuation : IDialogContinuation
{
    private readonly ConcurrentDictionary<long, ContinuationState> _store = new();

    public ContinuationState? Find(ParticipantId participantId)
    {
        if (participantId is null)
            throw new ArgumentNullException(nameof(participantId));

        return _store.TryGetValue(participantId.Value, out var value) ? value : null;
    }

    public void Begin(ParticipantId participantId, ContinuationState continuationState)
    {
        if (participantId is null)
            throw new ArgumentNullException(nameof(participantId));

        _store.AddOrUpdate(participantId.Value, continuationState, (p, v) => continuationState);
    }

    public void End(ParticipantId participantId, ContinuationState continuationState)
    {
        if (participantId is null)
            throw new ArgumentNullException(nameof(participantId));

        _store.TryRemove(new (participantId.Value, continuationState));
    }
}