namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class StoryForEstimate
{
    public Participant Participant { get; }
    public int StoryExternalId { get; }
    public AssessmentValue.Value Value { get; private set; }

    public StoryForEstimate(Participant participant, int storyExternalId)
    {
		Participant = participant ?? throw new ArgumentNullException(nameof(participant));
		StoryExternalId = storyExternalId;
		Value = AssessmentValue.Value.None;
	}

	public void SetValue(AssessmentValue.Value value) => Value = value;

	internal void Reset() => Value = AssessmentValue.Value.None;
}