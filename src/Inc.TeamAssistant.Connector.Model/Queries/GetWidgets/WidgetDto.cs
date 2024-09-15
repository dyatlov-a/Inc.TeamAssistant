namespace Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

public sealed record WidgetDto(
    string Type,
    int Position,
    bool CanEnabled,
    bool IsEnabled);