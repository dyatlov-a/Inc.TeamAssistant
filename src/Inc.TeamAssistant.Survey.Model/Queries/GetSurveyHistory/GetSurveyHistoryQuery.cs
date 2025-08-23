using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyHistory;

public sealed record GetSurveyHistoryQuery(Guid SurveyId, int Limit)
    : IRequest<GetSurveyHistoryResult>;