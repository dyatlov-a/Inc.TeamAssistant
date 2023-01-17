namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class Player
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public Person Person { get; private set; } = default!;

    private Player()
    {
    }

    public Player(Person person, Guid teamId)
        : this()
    {
        Person = person ?? throw new ArgumentNullException(nameof(person));
        Id = Guid.NewGuid();
        TeamId = teamId;
    }

    public static Player Build(Guid id, Guid teamId, Person person)
    {
        return new()
        {
            Id = id,
            TeamId = teamId,
            Person = person ?? throw new ArgumentNullException(nameof(person))
        };
    }
}