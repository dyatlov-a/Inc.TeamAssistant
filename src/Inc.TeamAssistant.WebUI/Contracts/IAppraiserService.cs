using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IAppraiserService
{
	Task<ServiceResult<GetActiveStoryResult>> GetActiveStory(Guid teamId, CancellationToken token = default);
	
	Task<ServiceResult<GetAssessmentHistoryResult>> GetAssessmentHistory(
		Guid teamId,
		DateOnly? from,
		CancellationToken token = default);
	
	Task<ServiceResult<GetStoriesResult>> GetStories(
		Guid teamId,
		DateOnly assessmentDate,
		CancellationToken token = default);
}