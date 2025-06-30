using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.CommandHandlers.StartSurvey;

internal sealed class StartSurveyCommandHandler : IRequestHandler<StartSurveyCommand>
{
    private readonly ISurveyReader _reader;
    private readonly ISurveyRepository _repository;

    public StartSurveyCommandHandler(ISurveyReader reader, ISurveyRepository repository)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task Handle(StartSurveyCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var templates = await _reader.GetTemplates(token);
        var template = templates.Single(t => t.Id == command.TemplateId);
        var survey = new SurveyEntry(Guid.CreateVersion7(), command.RoomId, DateTimeOffset.UtcNow, template);

        await _repository.Upsert(survey, token);
    }
}