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
    private readonly ISurveyRepository _repository;
    private readonly IPersonResolver _personResolver;

    public SetAnswerCommandHandler(ISurveyRepository repository, IPersonResolver personResolver)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
    }

    public async Task Handle(SetAnswerCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _personResolver.GetCurrentPerson();
        var existAnswer = await _repository.Find(command.SurveyId, currentPerson.Id, token);
        var survey = await command.SurveyId.Required(_repository.Find, token);

        if (!survey.QuestionIds.Contains(command.QuestionId))
            throw new TeamAssistantException($"Survey {command.SurveyId} not contains question {command.QuestionId}.");
        
        var answer = existAnswer ?? new SurveyAnswer(
            Guid.NewGuid(),
            command.SurveyId,
            DateTimeOffset.UtcNow,
            currentPerson.Id);
        answer.SetAnswer(new Answer(command.QuestionId, command.Value, command.Comment));

        await _repository.Upsert(answer, token);
    }
}