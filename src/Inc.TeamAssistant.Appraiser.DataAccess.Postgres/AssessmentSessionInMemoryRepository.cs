using System.Collections.Concurrent;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.DataAccess.Postgres;

internal sealed class AssessmentSessionInMemoryRepository : IAssessmentSessionRepository
{
    private ConcurrentBag<AssessmentSession> _store = new ();

    public AssessmentSession? Find(AssessmentSessionId assessmentSessionId)
    {
        if (assessmentSessionId is null)
            throw new ArgumentNullException(nameof(assessmentSessionId));

        var assessmentSessions = _store.Where(i => i.Id == assessmentSessionId).ToArray();

        return assessmentSessions.Length switch
        {
            0 => default,
            1 => assessmentSessions[0],
            _ => throw new AppraiserUserException(
                Messages.ActiveSessionsByIdFound,
                assessmentSessions.Length,
                assessmentSessionId.Value)
        };
    }

    public AssessmentSession? Find(ParticipantId participantId)
    {
        if (participantId is null)
            throw new ArgumentNullException(nameof(participantId));

        var assessmentSessions = _store
            .Where(i => i.Participants.Any(a => a.Id.Equals(participantId)) || i.Moderator.Id.Equals(participantId))
            .ToArray();

        return assessmentSessions.Length switch
        {
            0 => default,
            1 => assessmentSessions[0],
            _ => throw new AppraiserUserException(
                Messages.ActiveSessionsByParticipantFound,
                assessmentSessions.Length,
                participantId.Value)
        };
    }

    public void Add(AssessmentSession assessmentSession)
    {
        if (assessmentSession is null)
            throw new ArgumentNullException(nameof(assessmentSession));

        _store.Add(assessmentSession);
    }

    public void Remove(AssessmentSession assessmentSession)
    {
        if (assessmentSession is null)
            throw new ArgumentNullException(nameof(assessmentSession));

        _store = new(_store.Where(s => !s.Equals(assessmentSession)));
    }
}