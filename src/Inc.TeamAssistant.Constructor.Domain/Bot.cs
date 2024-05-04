namespace Inc.TeamAssistant.Constructor.Domain;

public sealed class Bot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;

    private Bot()
    {
    }
    
    public Bot(Guid id, string name)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Id = id;
        Name = name;
    }
}