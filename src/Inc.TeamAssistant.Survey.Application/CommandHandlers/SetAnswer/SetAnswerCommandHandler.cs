using Inc.TeamAssistant.Primitives;
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
    private readonly ISurveyEventSender _surveyEventSender;
    private readonly IOnlinePersonStore _onlinePersonStore;

    public SetAnswerCommandHandler(
        ISurveyRepository repository,
        IPersonResolver personResolver,
        ISurveyEventSender surveyEventSender,
        IOnlinePersonStore onlinePersonStore)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _personResolver = personResolver ?? throw new ArgumentNullException(nameof(personResolver));
        _surveyEventSender = surveyEventSender ?? throw new ArgumentNullException(nameof(surveyEventSender));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
    }

    public async Task Handle(SetAnswerCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var currentPerson = _personResolver.GetCurrentPerson();
        var survey = await command.SurveyId.Required(_repository.Find, token);
        
        var answer = new SurveyAnswer(
            command.SurveyId,
            command.QuestionId,
            currentPerson.Id,
            DateTimeOffset.UtcNow,
            command.Value,
            command.Comment);

        await _repository.Upsert(answer, token);

        if (command.IsFinished)
        {
            _onlinePersonStore.SetTicket(RoomId.CreateForSurvey(survey.RoomId), currentPerson, command.IsFinished);

            await _surveyEventSender.SurveyStateChanged(survey.RoomId, currentPerson.Id, command.IsFinished);
        }
    }
}