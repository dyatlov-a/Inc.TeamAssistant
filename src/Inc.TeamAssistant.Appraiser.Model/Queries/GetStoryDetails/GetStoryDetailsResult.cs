namespace Inc.TeamAssistant.Appraiser.Model.Queries.GetStoryDetails;

public sealed record GetStoryDetailsResult(
	string Title,
	IReadOnlyCollection<string> Links,
    string CodeForConnect,
    bool StorySelected,
	IReadOnlyCollection<StoryForEstimateDto> StoryForEstimates,
	string Total);