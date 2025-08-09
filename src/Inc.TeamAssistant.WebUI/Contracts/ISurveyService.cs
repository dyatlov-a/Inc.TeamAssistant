using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface ISurveyService
{
    Task<GetSurveyStateResult> GetSurveyState(Guid roomId, CancellationToken token = default);

    Task Start(StartSurveyCommand command, CancellationToken token = default);
    
    Task Finish(FinishSurveyCommand command, CancellationToken token = default);
}