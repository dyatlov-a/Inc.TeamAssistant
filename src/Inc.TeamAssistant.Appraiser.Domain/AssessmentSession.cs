using Inc.TeamAssistant.Appraiser.Domain.Exceptions;
using Inc.TeamAssistant.Appraiser.Domain.States;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class AssessmentSession : IAssessmentSessionAccessor
{
    private AssessmentSessionState _state;
    public Guid Id { get; }
    public long ChatId { get; }
    public Participant Moderator { get; }
    public LanguageId LanguageId { get; private set; }
    public string Title { get; private set; }
    public Story CurrentStory { get; private set; }
	IStoryAccessor IAssessmentSessionAccessor.Story => CurrentStory;

    private readonly List<Participant> _participants = new();
    public IReadOnlyCollection<Participant> Participants => _participants;

    public AssessmentSession(long chatId, Participant moderator, LanguageId languageId)
	{
		Moderator = moderator ?? throw new ArgumentNullException(nameof(moderator));
        LanguageId = languageId ?? throw new ArgumentNullException(nameof(languageId));
		Id = Guid.NewGuid();
		ChatId = chatId;
		_state = new Draft(this);
		CurrentStory = Story.Empty;
		Title = string.Empty;
	}

	public void Activate(long moderatorId, string title) => _state.Activate(moderatorId, title);
    public void ChangeLanguage(long moderatorId, LanguageId languageId)
        => _state.ChangeLanguage(moderatorId, languageId);
	public void Connect(long participantId, string name) => _state.Connect(participantId, name);
	public void StartStorySelection(long moderatorId) => _state.StartStorySelection(moderatorId);
	public void StorySelected(long moderatorId, string storyTitle, IReadOnlyCollection<string> links)
		=> _state.StorySelected(moderatorId, storyTitle, links);
	public void Estimate(Participant participant, AssessmentValue.Value value) => _state.Estimate(participant, value);
	public void CompleteEstimate(long moderatorId) => _state.CompleteEstimate(moderatorId);
	public void Disconnect(long participantId) => _state.Disconnect(participantId);
	public void Reset(long moderatorId) => _state.Reset(moderatorId);
	public bool InProgress() => _state.IsProgress();
    public bool EstimateEnded() => _state.EstimateEnded();

    void IAssessmentSessionAccessor.AddParticipant(Participant participant)
	{
		if (participant is null)
			throw new ArgumentNullException(nameof(participant));

		_participants.Add(participant);
    }

	void IAssessmentSessionAccessor.RemoveParticipant(Participant participant)
	{
		if (participant is null)
			throw new ArgumentNullException(nameof(participant));

		_participants.Remove(participant);
    }

	IAssessmentSessionAccessor IAssessmentSessionAccessor.AsModerator(long participantId, string operationName)
	{
		if (!Moderator.Id.Equals(participantId))
			throw new AppraiserUserException(Messages.NoRightsToMakeOperation, operationName);
        if (string.IsNullOrWhiteSpace(operationName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(operationName));

        return this;
	}

	IAssessmentSessionAccessor IAssessmentSessionAccessor.ChangeTitle(string title)
	{
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));

		Title = title;

		return this;
	}

	IAssessmentSessionAccessor IAssessmentSessionAccessor.ChangeStory(
        string storyTitle,
        IReadOnlyCollection<string> links)
	{
		if (string.IsNullOrWhiteSpace(storyTitle))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(storyTitle));
        if (links is null)
            throw new ArgumentNullException(nameof(links));

		CurrentStory = new(storyTitle, links);

		return this;
	}

	void IAssessmentSessionAccessor.MoveToState(Func<IAssessmentSessionAccessor, AssessmentSessionState> nextStateFactory)
	{
		if (nextStateFactory is null)
			throw new ArgumentNullException(nameof(nextStateFactory));

        _state = nextStateFactory(this);
	}

    void IAssessmentSessionAccessor.ChangeLanguage(LanguageId languageId)
    {
        LanguageId = languageId ?? throw new ArgumentNullException(nameof(languageId));
    }
}