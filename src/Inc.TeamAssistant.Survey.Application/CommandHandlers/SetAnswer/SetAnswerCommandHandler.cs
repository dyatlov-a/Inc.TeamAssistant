using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.SetAnswer;

internal sealed class SetAnswerCommandHandler : IRequestHandler<SetAnswerCommand>
{
    private readonly ISurveyState _surveyState;
    private readonly ISurveyRepository _repository;
    private readonly IPersonResolver _personResolver;

    public SetAnswerCommandHandler(
        ISurveyState surveyState,
        ISurveyRepository repository,
        IPersonResolver personResolver)
    {
        _surveyState = surveyState ?? throw new ArgumentNullException(nameof(surveyState));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(SetAnswerCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _personResolver.GetCurrentPerson();
        var ownerId = currentPerson.Id;
        var answers = _surveyState.GetAll(command.SurveyId);
        var survey = await command.SurveyId.Required(_repository.Find, token);

        if (!survey.QuestionIds.Contains(command.QuestionId))
            throw new TeamAssistantException($"Survey {command.SurveyId} not contains question {command.QuestionId}.");
        
        var answer = answers.SingleOrDefault(a => a.OwnerId == ownerId) ?? new SurveyAnswer(
            Guid.NewGuid(),
            command.SurveyId,
            DateTimeOffset.UtcNow,
            ownerId);
        answer.SetAnswer(new Answer(command.QuestionId, command.Value, command.Comment));

        _surveyState.Set(answer);
        
        if (survey.QuestionIds.Count == answer.Answers.Count)
            await _repository.Upsert(answer, token);
    }
}