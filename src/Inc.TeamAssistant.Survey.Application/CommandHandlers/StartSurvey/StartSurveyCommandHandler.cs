using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Features.Tenants;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.StartSurvey;

internal sealed class StartSurveyCommandHandler : IRequestHandler<StartSurveyCommand>
{
    private readonly ISurveyReader _reader;
    private readonly ISurveyRepository _repository;
    private readonly IRoomPropertiesProvider _propertiesProvider;
    private readonly ISurveyEventSender _surveyEventSender;

    public StartSurveyCommandHandler(
        ISurveyReader reader,
        ISurveyRepository repository,
        IRoomPropertiesProvider propertiesProvider,
        ISurveyEventSender surveyEventSender)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _propertiesProvider = propertiesProvider ?? throw new ArgumentNullException(nameof(propertiesProvider));
        _surveyEventSender = surveyEventSender ?? throw new ArgumentNullException(nameof(surveyEventSender));
    }

    public async Task Handle(StartSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var properties = await _propertiesProvider.Get(command.RoomId, token);
        var template = await properties.SurveyTemplateId.Required(_reader.FindTemplate, token);
        var survey = new SurveyEntry(
            Guid.CreateVersion7(),
            command.RoomId,
            DateTimeOffset.UtcNow,
            template);

        await _repository.Upsert(survey, token);

        await _surveyEventSender.SurveyStarted(command.RoomId);
    }
}