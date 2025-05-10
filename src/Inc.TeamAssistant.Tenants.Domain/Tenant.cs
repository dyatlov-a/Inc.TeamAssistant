namespace Inc.TeamAssistant.Tenants.Domain;

public sealed class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public long OwnerId { get; private set; }
    
    private Tenant()
    {
    }

    public Tenant(Guid id, string name, long ownerId)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Name = name;
        OwnerId = ownerId;
    }
}