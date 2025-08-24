using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Application.Services;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveyHistory;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveyHistory;

internal sealed class GetSurveyHistoryQueryHandler : IRequestHandler<GetSurveyHistoryQuery, GetSurveyHistoryResult>
{
    private readonly ISurveyRepository _repository;
    private readonly SurveySummaryService _service;

    public GetSurveyHistoryQueryHandler(ISurveyRepository repository, SurveySummaryService service)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    
    public async Task<GetSurveyHistoryResult> Handle(GetSurveyHistoryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var survey = await query.SurveyId.Required(_repository.Find, token);
        
        if (survey.State != SurveyState.Completed)
            return GetSurveyHistoryResult.Empty;
        
        var surveySummary = await _service.GetSurveySummary(survey, query.Limit, token);

        return new GetSurveyHistoryResult(surveySummary);
    }
}