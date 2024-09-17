namespace Inc.TeamAssistant.Constructor.Domain;

public sealed class Bot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    public long OwnerId { get; private set; }
    public Guid CalendarId { get; private set; }
    
    private readonly List<Guid> _featureIds = new();
    public IReadOnlyCollection<Guid> FeatureIds => _featureIds;
    public IReadOnlyDictionary<string, string> Properties { get; private set; } = default!;
    public IReadOnlyCollection<string> SupportedLanguages { get; private set; } = default!;

    private Bot()
    {
    }
    
    public Bot(
        Guid id,
        string name,
        string token,
        long ownerId,
        Guid calendarId,
        IReadOnlyDictionary<string, string> properties,
        IReadOnlyCollection<Guid> featureIds,
        IReadOnlyCollection<string> supportedLanguages)
        : this()
    {
        ArgumentNullException.ThrowIfNull(featureIds);
        ArgumentNullException.ThrowIfNull(supportedLanguages);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        
        Id = id;
        Name = name;
        Token = token;
        OwnerId = ownerId;
        CalendarId = calendarId;
        Properties = properties ?? throw new ArgumentNullException(nameof(properties));
        ChangeFeatures(featureIds);
        ChangeSupportedLanguages(supportedLanguages);
    }

    public Bot ChangeName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        
        Name = value;

        return this;
    }
    
    public Bot ChangeToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        
        Token = token;

        return this;
    }
    
    public Bot ChangeCalendarId(Guid calendarId)
    {
        CalendarId = calendarId;
        
        return this;
    }
    
    public Bot ChangeFeatures(IReadOnlyCollection<Guid> featureIds)
    {
        ArgumentNullException.ThrowIfNull(featureIds);
        
        _featureIds.Clear();
        _featureIds.AddRange(featureIds);
        
        return this;
    }
    
    public Bot ChangeSupportedLanguages(IReadOnlyCollection<string> supportedLanguages)
    {
        ArgumentNullException.ThrowIfNull(supportedLanguages);

        SupportedLanguages = supportedLanguages.ToArray();
        
        return this;
    }
    
    public Bot ChangeProperties(IReadOnlyDictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);

        Properties = properties.ToDictionary();
        
        return this;
    }
}