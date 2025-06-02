using Inc.TeamAssistant.Retro.Model.Commands.ChangeActionItem;

namespace Inc.TeamAssistant.WebUI.Features.Retro;

public sealed class ActionItemViewModel
{
    public Guid Id { get; private set; }
    public Guid RetroItemId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public string Text { get; private set; }

    public ActionItemViewModel(Guid id, Guid retroItemId, DateTimeOffset created)
    {
        Id = id;
        RetroItemId = retroItemId;
        Created = created;
        Text = string.Empty;
    }
    
    public ActionItemViewModel ChangeText(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        Text = text;

        return this;
    }
    
    public ChangeActionItemCommand ToCommand(Guid teamId) => new(Id, RetroItemId, teamId, Text);
}