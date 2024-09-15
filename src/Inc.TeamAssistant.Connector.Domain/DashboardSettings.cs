namespace Inc.TeamAssistant.Connector.Domain;

public sealed class DashboardSettings
{
    public long PersonId { get; private set; }
    public Guid BotId { get; private set; }
    public ICollection<DashboardWidget> Widgets { get; private set; }

    private DashboardSettings()
    {
        Widgets = new List<DashboardWidget>();
    }

    public DashboardSettings(long personId, Guid botId, IReadOnlyCollection<DashboardWidget> widgets)
        : this()
    {
        ArgumentNullException.ThrowIfNull(widgets);
        
        PersonId = personId;
        BotId = botId;

        foreach (var widget in widgets)
            Widgets.Add(widget);
    }
}