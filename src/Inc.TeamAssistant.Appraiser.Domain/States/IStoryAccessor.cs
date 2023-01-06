using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal interface IStoryAccessor
{
	string Title { get; }

	void AddStoryForEstimate(StoryForEstimate storyForEstimate);
	void RemoveStoryForEstimate(ParticipantId participantId);
	void Estimate(ParticipantId participantId, AssessmentValue.Value value);
	void Reset();
	bool EstimateEnded();
}