using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetAverageByTeam;
using Inc.TeamAssistant.Reviewer.Model.Queries.GetHistoryByTeam;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IReviewService
{
    Task<ServiceResult<GetHistoryByTeamResult>> GetHistory(Guid teamId, int depth, CancellationToken token = default);

    Task<ServiceResult<GetAverageByTeamResult>> GetAverage(Guid teamId, int depth, CancellationToken token = default);
}