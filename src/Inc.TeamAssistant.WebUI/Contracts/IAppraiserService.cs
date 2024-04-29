using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryById;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IAppraiserService
{
	Task<ServiceResult<GetStoryDetailsResult?>> GetStoryDetails(Guid teamId, CancellationToken token = default);
	
	Task<ServiceResult<GetAssessmentHistoryResult?>> GetAssessmentHistory(
		Guid teamId,
		int depth,
		CancellationToken token = default);
	
	Task<ServiceResult<GetStoriesResult?>> GetStories(
		Guid teamId,
		DateOnly assessmentDate,
		CancellationToken token = default);
	
	Task<ServiceResult<GetStoryByIdResult?>> GetStoryById(Guid storyId, CancellationToken token = default);
}