using Inc.TeamAssistant.Appraiser.Domain.Exceptions;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class InProgress : AssessmentSessionState
{
	public InProgress(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}
	
	public override void Connect(long participantId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

		if (AssessmentSession.Participants.Any(p => p.Id == participantId))
			throw new AppraiserUserException(Messages.AppraiserConnectWithError, name, AssessmentSession.Title);

		var participant = new Participant(participantId, name);
		AssessmentSession.AddParticipant(participant);
		AssessmentSession.Story.AddStoryForEstimate(new(participant));
	}
	
	public override void Disconnect(long participantId)
	{
		if (AssessmentSession.Moderator.Id == participantId)
			throw new AppraiserUserException(Messages.ModeratorCannotDisconnectedFromSession, AssessmentSession.Title);

		var participant = AssessmentSession.Participants.Single(a => a.Id == participantId);
		AssessmentSession.Story.RemoveStoryForEstimate(participant.Id);
		AssessmentSession.RemoveParticipant(participant);
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

	public override void CompleteEstimate(long moderatorId)
    {
        AssessmentSession
			.AsModerator(moderatorId)
			.MoveToState(a => new Idle(a));
    }

	public override bool IsProgress() => true;

	public override bool EstimateEnded() => AssessmentSession.Story.EstimateEnded();
}