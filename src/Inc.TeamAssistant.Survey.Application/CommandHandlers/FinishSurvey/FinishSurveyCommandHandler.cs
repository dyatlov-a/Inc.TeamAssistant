using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.FinishSurvey;

internal sealed class FinishSurveyCommandHandler : IRequestHandler<FinishSurveyCommand>
{
    private readonly ISurveyRepository _repository;
    private readonly ISurveyReader _reader;
    private readonly IPersonState _personState;
    private readonly ISurveyEventSender _surveyEventSender;

    public FinishSurveyCommandHandler(
        ISurveyRepository repository,
        ISurveyReader reader,
        IPersonState personState,
        ISurveyEventSender surveyEventSender)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _personState = personState ?? throw new ArgumentNullException(nameof(personState));
        _surveyEventSender = surveyEventSender ?? throw new ArgumentNullException(nameof(surveyEventSender));
    }

    public async Task Handle(FinishSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var survey = await _reader.Find(command.RoomId, SurveyStateRules.Active, token);

        if (survey is not null)
        {
            await _repository.Upsert(survey.MoveToFinish(), token);
            
            _personState.Clear(RoomId.CreateForSurvey(command.RoomId));

            await _surveyEventSender.SurveyFinished(survey.RoomId);
        }
    }
}