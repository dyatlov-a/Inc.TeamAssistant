using MediatR;

namespace Inc.TeamAssistant.Survey.Model.Queries.GetSurveyTemplates;

public sealed record GetSurveyTemplatesQuery()
    : IRequest<GetSurveyTemplatesResult>;