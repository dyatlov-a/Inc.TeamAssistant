namespace Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

public sealed record GetWidgetsResult(IReadOnlyCollection<WidgetDto> Widgets);