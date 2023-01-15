namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class PlayerAsReviewer
{
    public Guid Id { get; private set; }
    public Person Person { get; private set; } = default!;

    private PlayerAsReviewer()
    {
    }

    public PlayerAsReviewer(Player player)
        : this()
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));

        Id = player.Id;
        Person = player.Person;
    }
    
    public PlayerAsReviewer Build(Person person)
    {
        Person = person ?? throw new ArgumentNullException(nameof(person));

        return this;
    }
}