using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
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
        var to = new DateTimeOffset(new DateOnly(now.Year, now.Month, now.Day), TimeOnly.MinValue, TimeSpan.Zero);
        var from = query.From is null
            ? (DateTimeOffset?)null
            : new DateTimeOffset(query.From.Value, TimeOnly.MinValue, TimeSpan.Zero);
        
        var stories = await _reader.GetStories(query.TeamId, to, from, token);
        
        var results = stories
            .GroupBy(h => new DateOnly(h.Created.Year, h.Created.Month, h.Created.Day))
            .Select(h => new AssessmentHistoryDto(h.Key, h.Count(), h.Sum(s => s.GetWeight())))
            .OrderByDescending(h => h.AssessmentDate)
            .Take(query.Limit ?? maxLimit)
            .ToArray();
        
        return new GetAssessmentHistoryResult(results);
    }
}