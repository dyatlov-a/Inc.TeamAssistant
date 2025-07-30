using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Model.Commands.FinishAnswer;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.FinishAnswer;

internal sealed class FinishAnswerCommandHandler : IRequestHandler<FinishAnswerCommand>
{
    private readonly ISurveyState _surveyState;
    private readonly ISurveyRepository _repository;
    private readonly IPersonResolver _personResolver;

    public FinishAnswerCommandHandler(
        ISurveyState surveyState,
        ISurveyRepository repository,
        IPersonResolver personResolver)
    {
        _surveyState = surveyState ?? throw new ArgumentNullException(nameof(surveyState));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(FinishAnswerCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var currentPerson = _personResolver.GetCurrentPerson();
        var answer = _surveyState.Get(command.SurveyId, currentPerson.Id);
        
        await _repository.Upsert(answer!, token);
    }
}