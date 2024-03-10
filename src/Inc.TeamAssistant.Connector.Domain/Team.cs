namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Team
{
    public Guid Id { get; private set; }
    public Guid BotId { get; private set; }
    public long ChatId { get; private set; }
    public long OwnerId { get; private set; }
    public string Name { get; private set; } = default!;
    
    private readonly List<Person> _teammates = new();
    public IReadOnlyCollection<Person> Teammates => _teammates;
    public IReadOnlyDictionary<string, string> Properties { get; private set; } = default!;

    private Team()
    {
    }
    
    public Team(Guid botId, long chatId, long ownerId, string name, IReadOnlyDictionary<string, string> properties)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Id = Guid.NewGuid();
        BotId = botId;
        ChatId = chatId;
        OwnerId = ownerId;
        Name = name;
        Properties = properties;
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

    public Team ChangeProperty(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        
        Properties = new Dictionary<string, string>(Properties)
        {
            [name] = value
        };;
        
        return this;
    }
}