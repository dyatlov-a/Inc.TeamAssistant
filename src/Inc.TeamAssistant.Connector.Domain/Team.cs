namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Team
{
    public Guid Id { get; private set; }
    public Guid BotId { get; private set; }
    public long ChatId { get; private set; }
    public string Name { get; private set; } = default!;
    
    private readonly List<Person> _teammates = new();
    public IReadOnlyCollection<Person> Teammates => _teammates;

    private Team()
    {
    }
    
    public Team(Guid botId, long chatId, string name)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Id = Guid.NewGuid();
        BotId = botId;
        ChatId = chatId;
        Name = name;
    }

    public Team AddTeammate(Person person)
    {
        if (person is null)
            throw new ArgumentNullException(nameof(person));

        _teammates.Add(person);

        return this;
    }

    public Team RemoveTeammate(long personId)
    {
        var person = _teammates.Single(p => p.Id == personId);

        _teammates.Remove(person);

        return this;
    }
}