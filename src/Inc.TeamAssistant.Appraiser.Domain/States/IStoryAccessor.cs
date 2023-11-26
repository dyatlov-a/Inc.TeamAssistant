namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal interface IStoryAccessor
{
	string Title { get; }

	void AddStoryForEstimate(StoryForEstimate storyForEstimate);
	void RemoveStoryForEstimate(long participantId);
	void Estimate(long participantId, AssessmentValue.Value value);
	void Reset();
	bool EstimateEnded();
}