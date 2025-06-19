using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Tenants.Domain;

public sealed class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public long OwnerId { get; private set; }
    public string Properties { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    private Room()
    {
        Properties = "{}";
    }

    public Room(Guid id, string name, long ownerId, Tenant tenant)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Name = name;
        OwnerId = ownerId;
        Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
    }
    
    public Room CheckRights(long personId)
    {
        if (OwnerId != personId && Tenant.OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }

    public Room ChangeName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;

        return this;
    }

    internal Room MapTenant(Tenant tenant)
    {
        Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));

        return this;
    }
}