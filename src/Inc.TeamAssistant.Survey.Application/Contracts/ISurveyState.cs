using Inc.TeamAssistant.Survey.Domain;

namespace Inc.TeamAssistant.Survey.Application.Contracts;

public interface ISurveyState
{
    IReadOnlyCollection<SurveyAnswer> GetAll(Guid surveyId);

    void Set(SurveyAnswer surveyAnswer);

    void Clear(Guid surveyId);
}