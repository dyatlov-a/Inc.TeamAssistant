using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Tenants.Domain;

public sealed class Team
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public long OwnerId { get; private set; }
    public Tenant Tenant { get; private set; } = default!;

    private Team()
    {
    }

    public Team(Guid id, string name, long ownerId, Tenant tenant)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Name = name;
        OwnerId = ownerId;
        Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
    }
    
    public Team CheckRights(long personId)
    {
        if (OwnerId != personId && Tenant.OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }

    public Team ChangeName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;

        return this;
    }

    internal Team MapTenant(Tenant tenant)
    {
        Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));

        return this;
    }
}