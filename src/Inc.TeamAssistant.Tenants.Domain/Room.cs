using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Features.Tenants;

namespace Inc.TeamAssistant.Tenants.Domain;

public sealed class Room
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public long OwnerId { get; private set; }
    public RoomProperties Properties { get; private set; } = default!;
    public Tenant Tenant { get; private set; } = default!;

    private Room()
    {
    }

    public Room(Guid id, string name, long ownerId, RoomProperties properties, Tenant tenant)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Name = name;
        OwnerId = ownerId;
        Properties = properties ?? throw new ArgumentNullException(nameof(properties));
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