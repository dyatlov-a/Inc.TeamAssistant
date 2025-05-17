using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class RetroItemViewModel
{
    public Guid Id { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public long OwnerId { get; private set; }
    public Guid ColumnId { get; private set; }
    public int Position { get; private set; }
    public string? Text { get; private set; }
    public Guid? ParentId { get; private set; }
    
    private readonly List<RetroItemViewModel> _children = new();
    public IReadOnlyCollection<RetroItemViewModel> Children => _children;
    public string ParentKey => $"{Id:N}-parent";
    public string ChildKey => $"{Id:N}-child";

    public static RetroItemViewModel Create(Guid id, long ownerId, Guid columnId)
    {
        return new RetroItemViewModel
        {
            Id = id,
            Created = DateTimeOffset.UtcNow,
            OwnerId = ownerId,
            ColumnId = columnId
        };
    }
    
    public RetroItemViewModel Apply(RetroItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        Created = item.Created;
        Position = item.Position;
        ParentId = item.ParentId;
        
        ChangeText(item.Text);

        return this;
    }
    
    public RetroItemViewModel ChangeText(string? text)
    {
        Text = text;

        return this;
    }
    
    public RetroItemViewModel AddChild(RetroItemViewModel child)
    {
        ArgumentNullException.ThrowIfNull(child);

        child.ParentId = Id;
        _children.Add(child);

        return this;
    }
    
    public RetroItemViewModel ApplyChild(RetroItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var child = _children.SingleOrDefault(c => c.Id == item.Id);
        if (child is null)
            _children.Add(Create(item.Id, item.OwnerId, item.ColumnId).Apply(item));
        else
            child.Apply(item);

        return this;
    }

    public RetroItemViewModel MoveToSlot(Guid columnId, int maxColumnPosition)
    {
        ColumnId = columnId;
        Position = maxColumnPosition + 1;

        return this;
    }

    public RetroItemViewModel RemoveChild(RetroItemViewModel child)
    {
        child.ParentId = null;

        _children.Remove(child);

        return this;
    }

    public UpdateRetroItemCommand ToCommand() => new(Id, ColumnId, Position, Text, ParentId);
}