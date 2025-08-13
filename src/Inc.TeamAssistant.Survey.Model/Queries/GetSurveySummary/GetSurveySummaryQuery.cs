using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record GetSurveySummaryQuery(Guid RoomId, int Limit)
    : IRequest<GetSurveySummaryResult>;