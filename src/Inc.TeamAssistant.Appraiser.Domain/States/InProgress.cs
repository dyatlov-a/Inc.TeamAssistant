using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class InProgress : AssessmentSessionState
{
	public InProgress(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

	public override void Connect(ParticipantId participantId, string name)
	{
		if (participantId is null)
			throw new ArgumentNullException(nameof(participantId));
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

		if (AssessmentSession.Participants.Any(p => p.Id == participantId))
			throw new AppraiserUserException(Messages.AppraiserConnectWithError, name, AssessmentSession.Title);

		AssessmentSession.AddParticipant(new(participantId, name));
	}

    public override void Disconnect(ParticipantId participantId)
    {
        if (participantId is null)
            throw new ArgumentNullException(nameof(participantId));

        if (AssessmentSession.Moderator.Id == participantId)
            throw new AppraiserUserException(Messages.ModeratorCannotDisconnectedFromSession, AssessmentSession.Title);

        var appraiser = AssessmentSession.Participants.Single(a => a.Id == participantId);

        AssessmentSession.Story.RemoveStoryForEstimate(appraiser.Id);
        AssessmentSession.RemoveParticipant(appraiser);
    }

	public override void Estimate(Participant participant, AssessmentValue.Value value)
	{
		if (participant is null)
			throw new ArgumentNullException(nameof(participant));
		if (AssessmentSession.EstimateEnded())
			throw new AppraiserUserException(Messages.EstimateRejected, AssessmentSession.Story.Title);

		AssessmentSession.Story.Estimate(participant.Id, value);

		if (AssessmentSession.EstimateEnded())
			AssessmentSession.MoveToState(a => new Idle(a));
	}

    public override void AddStoryForEstimate(StoryForEstimate storyForEstimate)
    {
        if (storyForEstimate is null)
            throw new ArgumentNullException(nameof(storyForEstimate));

        AssessmentSession.Story.AddStoryForEstimate(storyForEstimate);
    }

    public override void CompleteEstimate(ParticipantId moderatorId)
    {
        if (moderatorId is null)
            throw new ArgumentNullException(nameof(moderatorId));

        AssessmentSession
			.AsModerator(moderatorId)
			.MoveToState(a => new Idle(a));
    }

	public override bool EstimateEnded() => AssessmentSession.Story.EstimateEnded();
}