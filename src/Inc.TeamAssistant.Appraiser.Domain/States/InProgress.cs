using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class InProgress : AssessmentSessionState
{
	public InProgress(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
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