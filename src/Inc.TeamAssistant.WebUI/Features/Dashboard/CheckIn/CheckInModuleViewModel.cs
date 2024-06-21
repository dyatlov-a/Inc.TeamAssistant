using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.CheckIn;

public sealed record CheckInModuleViewModel(string MapWidgetTitle)
    : IViewModel<CheckInModuleViewModel>
{
    public static CheckInModuleViewModel Empty { get; } = new(string.Empty);
}