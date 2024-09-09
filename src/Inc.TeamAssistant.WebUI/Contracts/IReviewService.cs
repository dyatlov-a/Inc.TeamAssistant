using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IReviewService
{
    Task<GetHistoryByTeamResult> GetHistory(Guid teamId, DateOnly from, CancellationToken token = default);

    Task<GetAverageByTeamResult> GetAverage(Guid teamId, DateOnly from, CancellationToken token = default);
    
    Task<GetLastTasksResult> GetLast(Guid teamId, DateOnly from, CancellationToken token = default);
}