using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class Idle : AssessmentSessionState
{
	public Idle(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
	{
	}

    public override void ChangeLanguage(long moderatorId, LanguageId languageId)
    {
        if (languageId is null)
            throw new ArgumentNullException(nameof(languageId));

        AssessmentSession
            .AsModerator(moderatorId)
            .ChangeLanguage(languageId);
    }

    public override void Connect(long participantId, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

		if (AssessmentSession.Participants.Any(p => p.Id == participantId))
			throw new AppraiserUserException(Messages.AppraiserConnectWithError, name, AssessmentSession.Title);

		var participant = new Participant(participantId, name);
		
		AssessmentSession.AddParticipant(participant);
		
		if (AssessmentSession.Story != Story.Empty)
			AssessmentSession.Story.AddStoryForEstimate(new (participant));
	}

	public override void Disconnect(long participantId)
	{
        if (AssessmentSession.Moderator.Id == participantId)
            throw new AppraiserUserException(Messages.ModeratorCannotDisconnectedFromSession, AssessmentSession.Title);

        var participant = AssessmentSession.Participants.Single(a => a.Id == participantId);
        
        if (AssessmentSession.Story != Story.Empty)
			AssessmentSession.Story.RemoveStoryForEstimate(participant.Id);
        
        AssessmentSession.RemoveParticipant(participant);
	}

	public override void StartStorySelection(long moderatorId)
	{
		AssessmentSession
			.AsModerator(moderatorId)
			.MoveToState(a => new StorySelection(a));
	}

	public override void Reset(long moderatorId)
	{
		AssessmentSession
			.AsModerator(moderatorId)
			.Story.Reset();

		AssessmentSession.MoveToState(a => new InProgress(a));
	}
}