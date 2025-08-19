using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Commands.StartSurvey;

public sealed record StartSurveyCommand(Guid RoomId)
    : IRequest;