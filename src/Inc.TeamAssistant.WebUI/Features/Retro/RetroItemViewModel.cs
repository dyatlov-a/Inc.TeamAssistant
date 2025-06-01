using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class RetroItemViewModel
{
    public Guid Id { get; private set; }
    public long OwnerId { get; private set; }
    public Guid ColumnId { get; private set; }
    public int Position { get; private set; }
    public string? Text { get; private set; }
    public Guid? ParentId { get; private set; }
    public int Votes { get; private set; }

    public RetroItemViewModel(Guid id, long ownerId)
    {
        Id = id;
        OwnerId = ownerId;
    }
    
    public bool HasOwnerRights(long ownerId) => OwnerId == ownerId;
    
    public RetroItemViewModel Apply(RetroItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        ColumnId = item.ColumnId;
        Position = item.Position;
        ParentId = item.ParentId;
        Votes = item.Votes;
        Text = item.Text;

        return this;
    }
    
    public RetroItemViewModel ChangeText(string? text)
    {
        Text = text;

        return this;
    }
    
    public RetroItemViewModel ChangeVotes(int votes)
    {
        Votes = votes;

        return this;
    }

    public UpdateRetroItemCommand ToCommand() => new(Id, ColumnId, Position, Text, ParentId);
}