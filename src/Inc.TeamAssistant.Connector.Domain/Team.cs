using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Team
{
    public Guid Id { get; private set; }
    public Guid BotId { get; private set; }
    public long ChatId { get; private set; }
    public Person Owner { get; private set; } = default!;
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
        Person owner,
        string name,
        IReadOnlyDictionary<string, string> properties)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        BotId = botId;
        ChatId = chatId;
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
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

    public string GetPropertyValueOrDefault(PropertyKey propertyKey, string defaultValue)
    {
        ArgumentNullException.ThrowIfNull(propertyKey);
        ArgumentNullException.ThrowIfNull(defaultValue);
        
        return Properties.GetValueOrDefault(propertyKey.Key, defaultValue);
    }

    public Team ChangeProperty(PropertyKey propertyKey, string value)
    {
        ArgumentNullException.ThrowIfNull(propertyKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        
        Properties = new Dictionary<string, string>(Properties)
        {
            [propertyKey.Key] = value
        };
        
        return this;
    }
    
    public Team RemoveProperty(params PropertyKey[] propertyKeys)
    {
        ArgumentNullException.ThrowIfNull(propertyKeys);

        var newProperties = Properties.Where(p => !propertyKeys.Any(n => n.Key.Equals(
            p.Key,
            StringComparison.InvariantCultureIgnoreCase)));
        
        Properties = new Dictionary<string, string>(newProperties);
        
        return this;
    }
    
    public sealed record PropertyKey
    {
        internal string Key { get; }

        public PropertyKey(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            
            Key = key;
        }
        
        public static readonly PropertyKey AccessToken = new("accessToken");
        public static readonly PropertyKey ProjectKey = new("projectKey");
        public static readonly PropertyKey ScrumMaster = new("scrumMaster");
    }
}