using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IAssessmentSessionRepository
{
    AssessmentSession? Find(AssessmentSessionId assessmentSessionId);

    AssessmentSession? Find(ParticipantId participantId);

    void Add(AssessmentSession assessmentSession);

    void Remove(AssessmentSession assessmentSession);
}