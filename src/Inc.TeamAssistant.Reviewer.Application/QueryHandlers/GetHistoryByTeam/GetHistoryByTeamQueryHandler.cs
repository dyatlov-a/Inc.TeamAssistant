using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.QueryHandlers.GetHistoryByTeam;

internal sealed class GetHistoryByTeamQueryHandler : IRequestHandler<GetHistoryByTeamQuery, GetHistoryByTeamResult>
{
    private readonly IReviewAnalyticsReader _reader;

    public GetHistoryByTeamQueryHandler(IReviewAnalyticsReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<GetHistoryByTeamResult> Handle(GetHistoryByTeamQuery query, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(query);

        var from = query.From.ToDateTimeOffset();
        var reviewHistory = await _reader.GetReviewHistory(query.TeamId, from, token);
        var requestsHistory = await _reader.GetRequestsHistory(query.TeamId, from, token);

        return new GetHistoryByTeamResult(reviewHistory, requestsHistory);
    }
}