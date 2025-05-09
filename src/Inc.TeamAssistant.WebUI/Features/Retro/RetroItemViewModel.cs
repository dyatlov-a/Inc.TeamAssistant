using Inc.TeamAssistant.Retro.Model.Commands.UpdateRetroItem;
using Inc.TeamAssistant.Retro.Model.Queries.GetRetroItems;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class RetroItemViewModel
{
    public Guid Id { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public long OwnerId { get; private set; }
    public int Type { get; private set; }
    public string? Text { get; set; }

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

    public RetroItemViewModel Apply(RetroItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Id = item.Id;
        Created = item.Created;
        OwnerId = item.OwnerId;
        Type = item.Type;
        Text = item.Text;

        return this;
    }

    public UpdateRetroItemCommand ToCommand() => new(Id, Text);
}