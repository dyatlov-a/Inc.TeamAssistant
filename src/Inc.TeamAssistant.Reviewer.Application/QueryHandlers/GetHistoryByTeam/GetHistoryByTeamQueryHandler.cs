using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetHistoryByTeam;

internal sealed class GetHistoryByTeamQueryHandler : IRequestHandler<GetHistoryByTeamQuery, GetHistoryByTeamResult>
{
    private readonly IReviewAnalyticsReader _reviewAnalyticsReader;

    public GetHistoryByTeamQueryHandler(IReviewAnalyticsReader reviewAnalyticsReader)
    {
        _reviewAnalyticsReader =
            reviewAnalyticsReader ?? throw new ArgumentNullException(nameof(reviewAnalyticsReader));
    }

    public async Task<GetHistoryByTeamResult> Handle(GetHistoryByTeamQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var from = new DateTimeOffset(query.From, TimeOnly.MinValue, TimeSpan.Zero);
        var reviewHistory = await _reviewAnalyticsReader.GetReviewHistory(query.TeamId, from, token);
        var requestsHistory = await _reviewAnalyticsReader.GetRequestsHistory(query.TeamId, from, token);

        return new GetHistoryByTeamResult(reviewHistory, requestsHistory);
    }
}