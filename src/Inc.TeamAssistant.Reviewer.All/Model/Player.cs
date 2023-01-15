namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class Player
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public long? LastReviewerId { get; private set; }
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

    public Player Build(Person person)
    {
        Person = person ?? throw new ArgumentNullException(nameof(person));

        return this;
    }
}