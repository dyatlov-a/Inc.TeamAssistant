using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class StorySelection : AssessmentSessionState
{
	public StorySelection(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

    public override void StorySelected(ParticipantId moderatorId, string storyTitle, IReadOnlyCollection<string> links)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));
		if (string.IsNullOrWhiteSpace(storyTitle))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));
        if (links is null)
            throw new ArgumentNullException(nameof(links));

		AssessmentSession
			  .AsModerator(moderatorId)
			  .ChangeStory(storyTitle, links)
			  .MoveToState(a => new InProgress(a));
	}
}