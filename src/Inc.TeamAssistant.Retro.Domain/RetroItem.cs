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
    public int? Votes { get; private set; }
    public Guid? ParentId { get; private set; }
    public IReadOnlyCollection<RetroItem> Children { get; private set; }

    private RetroItem()
    {
        Children = [];
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
    
    public RetroItem CheckCanChange(long personId)
    {
        var editAsOwner = RetroSession is null && OwnerId == personId;
        var editAsFacilitator = RetroSession?.HasRights(personId) == true;
        
        if (!editAsOwner && !editAsFacilitator)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }
    
    public RetroItem CheckCanRemove(long personId)
    {
        if (RetroSession is not null || OwnerId != personId)
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }

    public RetroItem ChangeText(string? value)
    {
        Text = value;

        return this;
    }
    
    public RetroItem ChangeParent(Guid? parentId)
    {
        if (ParentId != parentId)
        {
            ParentId = parentId;

            foreach (var child in Children)
                child.ChangeParent(parentId);
        }

        return this;
    }
    
    public RetroItem ChangePosition(Guid columnId, int position)
    {
        ColumnId = columnId;
        Position = position;

        foreach (var child in Children)
            child.ColumnId = columnId;

        return this;
    }

    public RetroItem SetVotes(int votes)
    {
        Votes = votes;

        return this;
    }

    internal RetroItem MapRetroSession(RetroSession? retroSession)
    {
        RetroSession = retroSession;

        return this;
    }
    
    internal RetroItem MapChildren(IReadOnlyCollection<RetroItem> children)
    {
        ArgumentNullException.ThrowIfNull(children);
        
        Children = children;

        return this;
    }
}