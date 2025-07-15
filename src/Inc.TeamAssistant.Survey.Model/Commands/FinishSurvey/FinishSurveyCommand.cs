using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Commands.FinishSurvey;

public sealed record FinishSurveyCommand(Guid SurveyId, Guid RoomId)
    : IRequest;