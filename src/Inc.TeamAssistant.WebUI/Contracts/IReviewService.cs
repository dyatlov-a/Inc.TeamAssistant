using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetLastTasks;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IReviewService
{
    Task<ServiceResult<GetHistoryByTeamResult>> GetHistory(Guid teamId, int depth, CancellationToken token = default);

    Task<ServiceResult<GetAverageByTeamResult>> GetAverage(Guid teamId, int depth, CancellationToken token = default);
    
    Task<ServiceResult<GetLastTasksResult>> GetLast(Guid teamId, int count, CancellationToken token = default);
}