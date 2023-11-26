using Inc.TeamAssistant.Appraiser.Domain;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public interface IAssessmentSessionRepository
{
    AssessmentSession? Find(Guid assessmentSessionId);

    AssessmentSession? Find(long participantId);

    void Add(AssessmentSession assessmentSession);

    void Remove(AssessmentSession assessmentSession);
}