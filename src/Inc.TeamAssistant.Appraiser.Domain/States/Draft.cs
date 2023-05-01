using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class Draft : AssessmentSessionState
{
	public Draft(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
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

    public override void Activate(ParticipantId moderatorId, string title)
	{
		if (moderatorId is null)
			throw new ArgumentNullException(nameof(moderatorId));
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

		AssessmentSession
			.AsModerator(moderatorId)
			.ChangeTitle(title)
			.MoveToState(a => new Idle(a));
	}
}