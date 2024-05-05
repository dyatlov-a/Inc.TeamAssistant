namespace Inc.TeamAssistant.Constructor.Domain;

public sealed class Bot
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Token { get; private set; } = default!;
    public long OwnerId { get; private set; }
    
    private readonly List<Guid> _featureIds = new();
    public IReadOnlyCollection<Guid> FeatureIds => _featureIds;

    private Bot()
    {
    }
    
    public Bot(Guid id, string name, string token, long ownerId)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(token));
        
        Id = id;
        Name = name;
        Token = token;
        OwnerId = ownerId;
    }
    
    public Bot AddFeature(Guid featureId)
    {
        if (!_featureIds.Contains(featureId))
            _featureIds.Add(featureId);
        
        return this;
    }
}