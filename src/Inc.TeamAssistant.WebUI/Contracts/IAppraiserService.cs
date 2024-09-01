using Inc.TeamAssistant.Appraiser.Model.Queries.GetAssessmentHistory;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetStories;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetActiveStory;

namespace Inc.TeamAssistant.WebUI.Contracts;

public interface IAppraiserService
{
	Task<GetActiveStoryResult> GetActiveStory(
		Guid teamId,
		string foreground,
		string background,
		CancellationToken token = default);
	
	Task<GetAssessmentHistoryResult> GetAssessmentHistory(
		Guid teamId,
		DateOnly? from,
		CancellationToken token = default);
	
	Task<GetStoriesResult> GetStories(
		Guid teamId,
		DateOnly assessmentDate,
		CancellationToken token = default);
}