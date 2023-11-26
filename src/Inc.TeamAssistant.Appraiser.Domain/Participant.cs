namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed class Participant
{
	public long Id { get; }
    public string Name { get; }

    public Participant(long id, string name)
    {
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

		Id = id;
		Name = name;
	}
}