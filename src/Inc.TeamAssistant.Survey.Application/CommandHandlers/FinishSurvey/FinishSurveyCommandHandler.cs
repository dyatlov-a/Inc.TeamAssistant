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
    private readonly ISurveyEventSender _surveyEventSender;
    private readonly IOnlinePersonStore _onlinePersonStore;

    public FinishSurveyCommandHandler(
        ISurveyRepository repository,
        ISurveyReader reader,
        ISurveyEventSender surveyEventSender,
        IOnlinePersonStore onlinePersonStore)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _surveyEventSender = surveyEventSender ?? throw new ArgumentNullException(nameof(surveyEventSender));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
    }

    public async Task Handle(FinishSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var survey = await _reader.Find(command.RoomId, SurveyStateRules.Active, token);

        if (survey is not null)
        {
            await _repository.Upsert(survey.MoveToFinish(), token);
            
            _onlinePersonStore.ClearTickets(RoomId.CreateForSurvey(command.RoomId));

            await _surveyEventSender.SurveyFinished(survey.RoomId);
        }
    }
}