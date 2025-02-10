using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

public sealed record GetWidgetsResult(IReadOnlyCollection<WidgetDto> Widgets)
    : IWithEmpty<GetWidgetsResult>
{
    public static GetWidgetsResult Empty { get; } = new(Array.Empty<WidgetDto>());
}