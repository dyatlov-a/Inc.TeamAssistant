using Inc.TeamAssistant.Appraiser.Domain.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class StorySelection : AssessmentSessionState
{
	public StorySelection(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}
	
	public override void Connect(long participantId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

		if (AssessmentSession.Participants.Any(p => p.Id == participantId))
			throw new AppraiserUserException(Messages.AppraiserConnectWithError, name, AssessmentSession.Title);

		AssessmentSession.AddParticipant(new(participantId, name));
	}
	
	public override void Disconnect(long participantId)
	{
		if (AssessmentSession.Moderator.Id == participantId)
			throw new AppraiserUserException(Messages.ModeratorCannotDisconnectedFromSession, AssessmentSession.Title);

		var appraiser = AssessmentSession.Participants.Single(a => a.Id == participantId);

		AssessmentSession.RemoveParticipant(appraiser);
	}

    public override void StorySelected(long moderatorId, string storyTitle, IReadOnlyCollection<string> links)
	{
		if (string.IsNullOrWhiteSpace(storyTitle))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));
        if (links is null)
            throw new ArgumentNullException(nameof(links));

		AssessmentSession
			  .AsModerator(moderatorId)
			  .ChangeStory(storyTitle, links)
			  .MoveToState(a => new InProgress(a));

		foreach (var participant in AssessmentSession.Participants)
			AssessmentSession.Story.AddStoryForEstimate(new(participant));
	}
}