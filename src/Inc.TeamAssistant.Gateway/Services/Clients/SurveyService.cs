using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyHistory;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;
using Inc.TeamAssistant.WebUI.Contracts;
using MediatR;

namespace Inc.TeamAssistant.Gateway.Services.Clients;

internal sealed class SurveyService : ISurveyService
{
    private readonly IMediator _mediator;

    public SurveyService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<GetSurveyStateResult> GetSurveyState(Guid roomId, CancellationToken token)
    {
        return await _mediator.Send(new GetSurveyStateQuery(roomId), token);
    }

    public async Task<GetSurveySummaryResult> GetSurveySummary(Guid roomId, int limit, CancellationToken token)
    {
        return await _mediator.Send(new GetSurveySummaryQuery(roomId, limit), token);
    }

    public async Task<GetSurveyHistoryResult> GetSurveyHistory(Guid surveyId, int limit, CancellationToken token)
    {
        return await _mediator.Send(new GetSurveyHistoryQuery(surveyId, limit), token);
    }

    public async Task Start(StartSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _mediator.Send(command, token);
    }

    public async Task Finish(FinishSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _mediator.Send(command, token);
    }
}