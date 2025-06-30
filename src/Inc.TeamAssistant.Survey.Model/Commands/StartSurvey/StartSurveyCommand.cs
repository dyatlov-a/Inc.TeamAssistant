using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;

public sealed record StartSurveyCommand(Guid RoomId, Guid TemplateId)
    : IRequest;