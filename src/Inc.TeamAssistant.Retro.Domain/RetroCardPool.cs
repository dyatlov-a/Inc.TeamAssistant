using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroCardPool
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public long OwnerId { get; private set; }

    private RetroCardPool()
    {
    }

    public RetroCardPool(Guid id, string name, long ownerId)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        
        Id = id;
        Name = name;
        OwnerId = ownerId;
    }
    
    public RetroCardPool CheckRights(long personId)
    {
        if (OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }

    public RetroCardPool ChangeName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;

        return this;
    }
}