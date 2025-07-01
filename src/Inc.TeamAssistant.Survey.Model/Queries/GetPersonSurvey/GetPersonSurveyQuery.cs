using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetPersonSurvey;

public sealed record GetPersonSurveyQuery(Guid SurveyId)
    : IRequest<GetPersonSurveyResult>;