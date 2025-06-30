using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyService
{
    Task<GetSurveyTemplatesResult> GetSurveyTemplates(CancellationToken token = default);

    Task StartSurvey(StartSurveyCommand command, CancellationToken token = default);
}