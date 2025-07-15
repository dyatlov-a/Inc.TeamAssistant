using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.FinishSurvey;

internal sealed class FinishSurveyCommandHandler : IRequestHandler<FinishSurveyCommand>
{
    private readonly ISurveyRepository _repository;

    public FinishSurveyCommandHandler(ISurveyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task Handle(FinishSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var survey = await command.SurveyId.Required(_repository.Find, token);

        await _repository.Upsert(survey.MoveToFinish(), token);
    }
}