using Inc.TeamAssistant.Connector.Model.Commands.UpdateWidgets;
using Inc.TeamAssistant.Connector.Model.Queries.GetWidgets;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Settings;

public sealed class DashboardSettingsFormModel
{
    private readonly List<DashboardSettingsItem> _items = new();
    public IReadOnlyCollection<DashboardSettingsItem> Items => _items;

    public DashboardSettingsFormModel Apply(IReadOnlyCollection<WidgetDto> widgets)
    {
        ArgumentNullException.ThrowIfNull(widgets);

        _items.Clear();
        _items.AddRange(widgets.Select(w => new DashboardSettingsItem
        {
            Type = w.Type,
            Feature = w.Feature,
            Position = w.Position,
            CanEnabled = w.CanEnabled,
            IsVisible = w.IsVisible
        }));

        return this;
    }

    public DashboardSettingsFormModel Swap(DashboardSettingsItem first, DashboardSettingsItem second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        (first.Position, second.Position) = (second.Position, first.Position);
        
        return this;
    }

    public UpdateWidgetsCommand ToCommand(Guid botId)
    {
        var items = _items.ToDictionary(i => i.Type, i => new UpdateWidgetDto(i.Position, i.IsVisible));
        
        return new UpdateWidgetsCommand(botId, items);
    }
}