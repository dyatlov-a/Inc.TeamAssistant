using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.SetAnswer;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.SetAnswer;

internal sealed class SetAnswerCommandHandler : IRequestHandler<SetAnswerCommand>
{
    private readonly ISurveyRepository _repository;
    private readonly IPersonResolver _personResolver;
    private readonly IPersonState _personState;
    private readonly ISurveyEventSender _surveyEventSender;

    public SetAnswerCommandHandler(
        ISurveyRepository repository,
        IPersonResolver personResolver,
        IPersonState personState,
        ISurveyEventSender surveyEventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _personState = personState ?? throw new ArgumentNullException(nameof(personState));
        _surveyEventSender = surveyEventSender ?? throw new ArgumentNullException(nameof(surveyEventSender));
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

        if (command.IsEnd)
        {
            var ticket = new PersonStateTicket(currentPerson, Finished: true, HandRaised: false);
            
            _personState.Set(RoomId.CreateForSurvey(survey.RoomId), ticket);

            await _surveyEventSender.SurveyStateChanged(survey.RoomId, ticket.Person.Id, ticket.Finished);
        }
    }
}