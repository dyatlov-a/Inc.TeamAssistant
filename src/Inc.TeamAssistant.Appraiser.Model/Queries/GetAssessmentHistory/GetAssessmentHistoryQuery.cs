using MediatR;

namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;

public sealed record GetAssessmentHistoryQuery(
    Guid TeamId,
    int? Limit,
    DateOnly? From)
    : IRequest<GetAssessmentHistoryResult>;