using Inc.TeamAssistant.Primitives;

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
    public IReadOnlyDictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>();

    private Team()
    {
    }
    
    public Team(
        Guid id,
        Guid botId,
        long chatId,
        long ownerId,
        string name,
        IReadOnlyDictionary<string, string> properties)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(name));
        
        Id = id;
        BotId = botId;
        ChatId = chatId;
        OwnerId = ownerId;
        Name = name;
        Properties = properties ?? throw new ArgumentNullException(nameof(properties));
    }

    public Team AddTeammate(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

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
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(name));
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(value));
        
        Properties = new Dictionary<string, string>(Properties)
        {
            [name] = value
        };
        
        return this;
    }
}