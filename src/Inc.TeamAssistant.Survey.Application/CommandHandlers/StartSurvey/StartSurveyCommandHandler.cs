using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.StartSurvey;

internal sealed class StartSurveyCommandHandler : IRequestHandler<StartSurveyCommand>
{
    private readonly ISurveyRepository _repository;
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly ISurveyEventSender _surveyEventSender;
    private readonly IOnlinePersonStore _onlinePersonStore;

    public StartSurveyCommandHandler(
        ISurveyRepository repository,
        IRoomPropertiesProvider propertiesProvider,
        ISurveyEventSender surveyEventSender,
        IOnlinePersonStore onlinePersonStore)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _surveyEventSender = surveyEventSender ?? throw new ArgumentNullException(nameof(surveyEventSender));
        _onlinePersonStore = onlinePersonStore ?? throw new ArgumentNullException(nameof(onlinePersonStore));
    }

    public async Task Handle(StartSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var properties = await _propertiesProvider.Get(command.RoomId, token);
        var survey = new SurveyEntry(
            Guid.CreateVersion7(),
            command.RoomId,
            properties.SurveyTemplateId,
            DateTimeOffset.UtcNow);

        await _repository.Upsert(survey, token);
        
        _onlinePersonStore.ClearTickets(RoomId.CreateForSurvey(command.RoomId));

        await _surveyEventSender.SurveyStarted(command.RoomId);
    }
}