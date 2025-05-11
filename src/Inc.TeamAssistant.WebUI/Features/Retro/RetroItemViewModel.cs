using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class RetroItemViewModel
{
    public Guid Id { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public long OwnerId { get; private set; }
    public int Type { get; private set; }
    public string? Text { get; private set; }

    public static RetroItemViewModel Create(Guid id, long ownerId, int type)
    {
        return new RetroItemViewModel
        {
            Id = id,
            Created = DateTimeOffset.UtcNow,
            OwnerId = ownerId,
            Type = type
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
        ChangeText(item.Text);

        return this;
    }

    public UpdateRetroItemCommand ToCommand() => new(Id, Text);
}