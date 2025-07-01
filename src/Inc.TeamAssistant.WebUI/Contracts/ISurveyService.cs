using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetPersonSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyService
{
    Task<GetSurveyTemplatesResult> GetTemplates(CancellationToken token = default);

    Task<GetPersonSurveyResult> GetPersonSurveys(Guid surveyId, CancellationToken token = default);

    Task Start(StartSurveyCommand command, CancellationToken token = default);
}