using Inc.TeamAssistant.Survey.Application.Contracts;
using Inc.TeamAssistant.Survey.Application.QueryHandlers.Services;
using Inc.TeamAssistant.Survey.Domain;
using Inc.TeamAssistant.Survey.Model.Queries.GetSurveySummary;
using MediatR;

namespace Inc.TeamAssistant.Survey.Application.QueryHandlers.GetSurveySummary;

internal sealed class GetSurveySummaryQueryHandler : IRequestHandler<GetSurveySummaryQuery, GetSurveySummaryResult>
{
    private readonly ISurveyReader _reader;
    private readonly SurveySummaryService _service;

    public GetSurveySummaryQueryHandler(ISurveyReader reader, SurveySummaryService service)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<GetSurveySummaryResult> Handle(GetSurveySummaryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var survey = await _reader.ReadLastSurvey(query.RoomId, [SurveyState.Completed], token);
        if (survey is null)
            return GetSurveySummaryResult.Empty;
        
        var surveySummary = await _service.GetSurveySummary(survey, top: query.Limit, token);

        return new GetSurveySummaryResult(surveySummary);
    }
}