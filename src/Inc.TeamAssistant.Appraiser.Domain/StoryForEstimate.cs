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

    public StoryForEstimate(Guid storyId, long participantId, string participantDisplayName)
		: this()
    {
	    if (string.IsNullOrWhiteSpace(participantDisplayName))
		    throw new ArgumentException("Value cannot be null or whitespace.", nameof(participantDisplayName));
	    
	    Id = Guid.NewGuid();
	    StoryId = storyId;
	    ParticipantId = participantId;
	    ParticipantDisplayName = participantDisplayName;
		Value = AssessmentValue.Value.None;
	}

	internal void SetValue(AssessmentValue.Value value) => Value = value;

	internal void Reset() => Value = AssessmentValue.Value.None;
}