namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class StoryForEstimate
{
	public Guid Id { get; private set; }
	public Guid StoryId { get; private set; }
    public long ParticipantId { get; private set; }
    public string ParticipantDisplayName { get; private set; } = default!;
    public AssessmentValue.Value Value { get; private set; }

    private StoryForEstimate()
    {
    }

    public StoryForEstimate(Guid id, Guid storyId, long participantId, string participantDisplayName)
		: this()
    {
	    ArgumentException.ThrowIfNullOrWhiteSpace(participantDisplayName);
	    
	    Id = id;
	    StoryId = storyId;
	    ParticipantId = participantId;
	    ParticipantDisplayName = participantDisplayName;
		Value = AssessmentValue.Value.None;
	}

    internal bool SetValue(AssessmentValue.Value value)
    {
	    var alreadyHasValue = Value != AssessmentValue.Value.None;
	    
	    Value = value;

	    return alreadyHasValue;
    }

	internal void Reset() => Value = AssessmentValue.Value.None;
}