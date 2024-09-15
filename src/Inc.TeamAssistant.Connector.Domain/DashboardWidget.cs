namespace Inc.TeamAssistant.Connector.Domain;

public sealed record DashboardWidget(
    string Type,
    int Position,
    bool CanEnabled,
    bool IsEnabled);