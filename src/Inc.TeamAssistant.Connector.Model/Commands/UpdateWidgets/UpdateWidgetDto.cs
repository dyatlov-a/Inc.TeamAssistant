namespace Inc.TeamAssistant.Connector.Model.Commands.UpdateWidgets;

public sealed record UpdateWidgetDto(
    int Position,
    bool IsEnabled);