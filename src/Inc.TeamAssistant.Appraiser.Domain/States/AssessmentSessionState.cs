using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal abstract class AssessmentSessionState
{
	protected readonly IAssessmentSessionAccessor AssessmentSession;

	protected AssessmentSessionState(IAssessmentSessionAccessor assessmentSession)
		=> AssessmentSession = assessmentSession ?? throw new ArgumentNullException(nameof(assessmentSession));

	public virtual void Activate(long moderatorId, string title) => Throw();
    public virtual void ChangeLanguage(long moderatorId, LanguageId languageId) => Throw();
	public virtual void Connect(long participantId, string name) => Throw();
	public virtual void StartStorySelection(long moderatorId) => Throw();
	public virtual void StorySelected(long moderatorId, string storyTitle, IReadOnlyCollection<string> links)
        => Throw();
	public virtual void Estimate(Participant participant, AssessmentValue.Value value) => Throw();
	public virtual void CompleteEstimate(long moderatorId) => Throw();
	public virtual void Reset(long moderatorId) => Throw();
	public virtual void Disconnect(long participantId) => Throw();
	public virtual bool EstimateEnded() => true;
	public virtual bool IsProgress() => false;

	private void Throw() => throw new AppraiserUserException(Messages.NotValidState, GetType().Name);
}