namespace Inc.TeamAssistant.Connector.Domain;

public sealed class DashboardWidget
{
    public string Feature { get; private set; } = default!;
    public string Type { get; private set; } = default!;
    public int Position { get; private set; }
    public bool IsEnabled { get; private set; }

    private DashboardWidget()
    {
    }

    public DashboardWidget(string feature, string type, int position, bool isEnabled)
        : this()
    {
        ArgumentNullException.ThrowIfNull(feature);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        Feature = feature;
        Type = type;
        Position = position;
        IsEnabled = isEnabled;
    }

    internal DashboardWidget Change(int position, bool isEnabled)
    {
        Position = position;
        IsEnabled = isEnabled;

        return this;
    }
}