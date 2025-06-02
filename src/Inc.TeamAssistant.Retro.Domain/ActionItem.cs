namespace Inc.TeamAssistant.Retro.Domain;

public sealed class ActionItem
{
    public Guid Id { get; private set; }
    public Guid RetroItemId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public string Text { get; private set; } = default!;
    public ActionItemState State { get; private set; }

    private ActionItem()
    {
    }

    public ActionItem(
        Guid id,
        Guid retroItemId,
        DateTimeOffset created)
        : this()
    {
        Id = id;
        RetroItemId = retroItemId;
        Created = created;
        State = ActionItemState.New;
    }
    
    public ActionItem ChangeText(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        Text = text;
        
        return this;
    }

    public ActionItem ChangeState(ActionItemState state)
    {
        State = state;

        return this;
    }
}