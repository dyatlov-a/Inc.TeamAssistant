using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal sealed class Draft : AssessmentSessionState
{
	public Draft(IAssessmentSessionAccessor assessmentSession) : base(assessmentSession)
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

    public override void Activate(long moderatorId, string title)
	{
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

		AssessmentSession
			.AsModerator(moderatorId)
			.ChangeTitle(title)
			.MoveToState(a => new Idle(a));
	}
}