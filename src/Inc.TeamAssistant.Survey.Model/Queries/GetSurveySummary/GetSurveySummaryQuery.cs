using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;

public sealed record GetSurveySummaryQuery(Guid SurveyId, int Limit)
    : IRequest<GetSurveySummaryResult>;