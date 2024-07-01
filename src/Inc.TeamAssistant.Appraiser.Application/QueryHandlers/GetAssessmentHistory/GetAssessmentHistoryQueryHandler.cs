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

        var now = DateTimeOffset.UtcNow;
        var before = new DateTimeOffset(new DateOnly(now.Year, now.Month, now.Day), TimeOnly.MinValue, TimeSpan.Zero);
        var history = await _reader.GetAssessmentHistory(query.TeamId, before, query.Depth, token);

        return new GetAssessmentHistoryResult(history);
    }
}