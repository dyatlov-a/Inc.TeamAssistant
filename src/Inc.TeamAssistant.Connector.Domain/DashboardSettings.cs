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

    public DashboardSettings ChangeWidget(string type, int position, bool isEnabled)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        
        var widget = Widgets.Single(w => w.Type == type);
        
        widget.Change(position, isEnabled);

        return this;
    }
    
    public static DashboardSettings CreateDefaultSettings(long personId, Guid botId)
    {
        return new DashboardSettings(personId, botId, [
            new(string.Empty, "TeammatesWidget", 1, true),
            new("Reviewer", "ReviewTotalStatsWidget", 2, true),
            new("Reviewer", "ReviewHistoryWidget", 3, true),
            new("Reviewer", "ReviewAverageStatsWidget", 4, true),
            new("Appraiser", "AppraiserHistoryWidget", 5, true),
            new("Appraiser", "AppraiserIntegrationWidget", 6, true),
            new("RandomCoffee", "RandomCoffeeHistoryWidget", 7, true),
            new("CheckIn", "MapWidget", 8, true)
        ]);
    }
}