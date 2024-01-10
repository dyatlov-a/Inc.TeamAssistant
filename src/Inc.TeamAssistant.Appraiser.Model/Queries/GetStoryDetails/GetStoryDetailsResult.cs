namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

public sealed record GetStoryDetailsResult(
	string AssessmentSessionTitle,
    string CodeForConnect,
    bool StorySelected,
    StoryDetails Story,
	IReadOnlyCollection<StoryForEstimateDto> StoryForEstimates,
	string Total);