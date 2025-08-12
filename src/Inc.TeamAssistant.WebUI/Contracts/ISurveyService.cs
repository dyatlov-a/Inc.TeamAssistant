using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyService
{
    Task<GetSurveyStateResult> GetSurveyState(Guid roomId, CancellationToken token = default);

    Task<GetSurveySummaryResult> GetSurveySummary(Guid surveyId, int limit, CancellationToken token = default);

    Task Start(StartSurveyCommand command, CancellationToken token = default);
    
    Task Finish(FinishSurveyCommand command, CancellationToken token = default);
}