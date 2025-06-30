using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;
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
    
    public async Task<GetSurveyTemplatesResult> GetSurveyTemplates(CancellationToken token)
    {
        return await _mediator.Send(new GetSurveyTemplatesQuery(), token);
    }

    public async Task StartSurvey(StartSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _mediator.Send(command, token);
    }
}