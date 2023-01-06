using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain.States;

internal abstract class AssessmentSessionState
{
	protected readonly IAssessmentSessionAccessor AssessmentSession;

	protected AssessmentSessionState(IAssessmentSessionAccessor assessmentSession)
		=> AssessmentSession = assessmentSession ?? throw new ArgumentNullException(nameof(assessmentSession));

	public virtual void Activate(ParticipantId moderatorId, string title) => Throw();
    public virtual void ChangeLanguage(ParticipantId moderatorId, LanguageId languageId) => Throw();
	public virtual void Connect(ParticipantId participantId, string name) => Throw();
	public virtual void StartStorySelection(ParticipantId moderatorId) => Throw();
	public virtual void StorySelected(ParticipantId moderatorId, string storyTitle, IReadOnlyCollection<string> links)
        => Throw();
	public virtual void AddStoryForEstimate(StoryForEstimate storyForEstimate) => Throw();
	public virtual void Estimate(Participant participant, AssessmentValue.Value value) => Throw();
	public virtual void CompleteEstimate(ParticipantId moderatorId) => Throw();
	public virtual void Reset(ParticipantId moderatorId) => Throw();
	public virtual void Disconnect(ParticipantId participantId) => Throw();
	public virtual bool EstimateEnded() => true;

	private void Throw() => throw new AppraiserUserException(Messages.NotValidState, GetType().Name);
}