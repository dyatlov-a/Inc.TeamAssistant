namespace Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

public sealed record WidgetDto(
    string Type,
    string Feature,
    int Position,
    bool CanEnabled,
    bool IsEnabled)
{
    public bool IsVisible => CanEnabled && IsEnabled;
}