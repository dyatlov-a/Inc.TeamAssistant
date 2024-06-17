using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

namespace Inc.TeamAssistant.Reviewer.Application.Contracts;

public interface IReviewAnalyticsReader
{
    Task<IReadOnlyCollection<HistoryByTeamItemDto>> GetReviewHistory(
        Guid teamId,
        DateTimeOffset from,
        CancellationToken token);
    
    Task<IReadOnlyCollection<HistoryByTeamItemDto>> GetRequestsHistory(
        Guid teamId,
        DateTimeOffset from,
        CancellationToken token);
}