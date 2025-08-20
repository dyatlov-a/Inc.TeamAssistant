using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class ActionItemViewModel
{
    public Guid Id { get; private set; }
    public Guid RetroItemId { get; private set; }
    public string Text { get; private set; }
    public string State { get; private set; }
    public DateTimeOffset Modified { get; private set; }

    public ActionItemViewModel(Guid id, Guid retroItemId, DateTimeOffset created)
    {
        Id = id;
        RetroItemId = retroItemId;
        Text = string.Empty;
        State = ActionItemStages.New;
        Modified = created;
    }
    
    public ActionItemViewModel ChangeText(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        Text = text;

        return this;
    }

    public ActionItemViewModel MoveToDone()
    {
        State = ActionItemStages.Done;
        Modified = DateTimeOffset.UtcNow;
        
        return this;
    }
    
    public ActionItemViewModel MoveToPinned()
    {
        State = ActionItemStages.Pinned;
        Modified = DateTimeOffset.UtcNow;
        
        return this;
    }

    public ActionItemViewModel Apply(ActionItemDto item)
    {
        ArgumentNullException.ThrowIfNull(item);

        Text = item.Text;
        State = item.State;
        Modified = item.Modified ?? item.Created;
        
        return this;
    }
    
    public ChangeActionItemCommand ToCommand(Guid roomId, bool notify = false) => new(
        Id,
        RetroItemId,
        roomId,
        Text,
        State,
        notify);
}