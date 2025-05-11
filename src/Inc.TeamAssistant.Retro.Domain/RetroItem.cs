using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroItem
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public string Type { get; private set; } = default!;
    public string? Text { get; private set; }
    public long OwnerId { get; private set; }

    private RetroItem()
    {
    }

    public RetroItem(
        Guid id,
        Guid teamId,
        DateTimeOffset now,
        string type,
        string? text,
        long ownerId)
        : this()
    {
        Id = id;
        TeamId = teamId;
        Created = now;
        Type = type;
        Text = text;
        OwnerId = ownerId;
    }
    
    public RetroItem CheckRights(long personId)
    {
        if (OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }

    public RetroItem ChangeText(string? value)
    {
        Text = value;

        return this;
    }
}