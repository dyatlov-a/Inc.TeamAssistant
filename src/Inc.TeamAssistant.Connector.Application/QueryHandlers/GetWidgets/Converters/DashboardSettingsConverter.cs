using Inc.TeamAssistant.Connector.Domain;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetWidgets.Converters;

internal static class DashboardSettingsConverter
{
    public static IReadOnlyCollection<WidgetDto> Convert(
        DashboardSettings settings,
        IReadOnlyCollection<string> features)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(features);
        
        return settings.Widgets
            .Select(w => new WidgetDto(
                w.Type,
                w.Feature,
                w.Position,
                CanEnabled(w), w.IsEnabled))
            .ToArray();
        
        bool CanEnabled(DashboardWidget widget) =>
            string.IsNullOrWhiteSpace(widget.Feature) ||
            features.Contains(widget.Feature, StringComparer.InvariantCultureIgnoreCase);
    }
}