namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class StoryForEstimate
{
	public Guid Id { get; private set; }
	public Guid StoryId { get; private set; }
    public long ParticipantId { get; private set; }
    public string ParticipantDisplayName { get; private set; } = default!;
    public int Value { get; private set; }

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
		Value = Estimation.None.Value;
	}

    internal bool SetValue(Estimation estimation)
    {
	    ArgumentNullException.ThrowIfNull(estimation);
	    
	    var alreadyHasValue = Value != Estimation.None.Value;
	    
	    Value = estimation.Value;

	    return alreadyHasValue;
    }

	internal void Reset() => Value = Estimation.None.Value;
}