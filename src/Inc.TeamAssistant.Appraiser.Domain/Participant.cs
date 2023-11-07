using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class Participant
{
	public ParticipantId Id { get; }
    public string Name { get; }

    public Participant(ParticipantId id, string name)
    {
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

		Id = id ?? throw new ArgumentNullException(nameof(id));
		Name = name;
	}
}