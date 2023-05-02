using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class Idle : AssessmentSessionState
{
	public Idle(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

    public override void ChangeLanguage(ParticipantId moderatorId, LanguageId languageId)
    {
        if (moderatorId is null)
            throw new ArgumentNullException(nameof(moderatorId));
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));

        AssessmentSession
            .AsModerator(moderatorId)
            .ChangeLanguage(languageId);
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

        var participant = AssessmentSession.Participants.Single(a => a.Id == participantId);
        
        if (AssessmentSession.Story != Story.Empty)
			AssessmentSession.Story.RemoveStoryForEstimate(participant.Id);
        
        AssessmentSession.RemoveParticipant(participant);
	}

	public override void StartStorySelection(ParticipantId moderatorId)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));

		AssessmentSession
			.AsModerator(moderatorId)
			.MoveToState(a => new StorySelection(a));
	}

	public override void Reset(ParticipantId moderatorId)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));

		AssessmentSession
			.AsModerator(moderatorId)
			.Story.Reset();

		AssessmentSession.MoveToState(a => new InProgress(a));
	}
}