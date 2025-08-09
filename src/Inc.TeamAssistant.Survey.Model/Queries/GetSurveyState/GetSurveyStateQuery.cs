using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyState;

public sealed record GetSurveyStateQuery(Guid RoomId)
    : IRequest<GetSurveyStateResult>;