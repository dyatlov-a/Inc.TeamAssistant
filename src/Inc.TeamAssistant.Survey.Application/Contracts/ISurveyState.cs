using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyState
{
    SurveyAnswer? Get(Guid surveyId, long ownerId);

    void Set(SurveyAnswer surveyAnswer);

    void Clear(Guid surveyId);
}