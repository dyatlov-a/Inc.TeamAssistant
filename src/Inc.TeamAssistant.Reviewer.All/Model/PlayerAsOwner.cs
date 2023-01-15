namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class PlayerAsOwner
{
    public Guid Id { get; private set; }
    public Person Person { get; private set; } = default!;
    public long LastReviewerId { get; private set; }

    private PlayerAsOwner()
    {
    }

    public PlayerAsOwner(Player player, long lastReviewerId)
        : this()
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));
        
        Id = player.Id;
        Person = player.Person;
        LastReviewerId = lastReviewerId;
    }
    
    public PlayerAsOwner Build(Person person)
    {
        Person = person ?? throw new ArgumentNullException(nameof(person));

        return this;
    }
}