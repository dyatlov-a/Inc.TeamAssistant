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
    
    public RetroItemViewModel ChangeText(string? text)
    {
        Text = text;

        return this;
    }

    public RetroItemViewModel Apply(RetroItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        Created = item.Created;
        Position = item.Position;
        
        ChangeText(item.Text);

        return this;
    }

    public UpdateRetroItemCommand ToCommand() => new(Id, Text);
}