using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetAssessmentHistory.Converters;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Primitives.Extensions;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetAssessmentHistory;

internal sealed class GetAssessmentHistoryQueryHandler
    : IRequestHandler<GetAssessmentHistoryQuery, GetAssessmentHistoryResult>
{
    private readonly IStoryReader _reader;

    public GetAssessmentHistoryQueryHandler(IStoryReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetAssessmentHistoryResult> Handle(GetAssessmentHistoryQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        const int maxLimit = 365;
        
        var now = DateTimeOffset.UtcNow;
        var to = new DateOnly(now.Year, now.Month, now.Day).ToDateTimeOffset();
        var from = query.From?.ToDateTimeOffset();
        var stories = await _reader.GetStories(query.TeamId, to, from, token);
        var results = AssessmentHistoryDtoConverter.ConvertFrom(stories, query.Limit ?? maxLimit);
        
        return new GetAssessmentHistoryResult(results);
    }
}