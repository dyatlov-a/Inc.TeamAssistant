using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;
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