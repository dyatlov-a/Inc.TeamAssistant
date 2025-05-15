using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroItem
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public Guid ColumnId { get; private set; }
    public int Position { get; private set; }
    public string? Text { get; private set; }
    public long OwnerId { get; private set; }
    public Guid? RetroSessionId { get; private set; }
    public RetroSession? RetroSession { get; private set; }
    public Guid? ParentId { get; private set; }

    private RetroItem()
    {
    }

    public RetroItem(
        Guid id,
        Guid teamId,
        DateTimeOffset now,
        Guid columnId,
        int position,
        string? text,
        long ownerId)
        : this()
    {
        Id = id;
        TeamId = teamId;
        Created = now;
        ColumnId = columnId;
        Position = position;
        Text = text;
        OwnerId = ownerId;
    }
    
    public RetroItem CheckRights(long personId)
    {
        var editAsOwner = RetroSession is null && OwnerId != personId;
        var editAsFacilitator = RetroSession is not null && RetroSession.FacilitatorId != personId;
        
        if (editAsOwner || editAsFacilitator)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }

    public RetroItem ChangeText(string? value)
    {
        Text = value;

        return this;
    }
    
    public RetroItem AttachToSession(Guid sessionId)
    {
        RetroSessionId = sessionId;

        return this;
    }
    
    public RetroItem ChangeParent(Guid? parentId)
    {
        ParentId = parentId;

        return this;
    }

    internal RetroItem MapRetroSession(RetroSession? retroSession)
    {
        RetroSession = retroSession;

        return this;
    }
}