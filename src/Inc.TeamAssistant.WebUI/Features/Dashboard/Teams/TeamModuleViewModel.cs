using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Teams;

public sealed record TeamModuleViewModel(string TeammatesWidgetTitle)
    : IViewModel<TeamModuleViewModel>
{
    public static TeamModuleViewModel Empty { get; } = new(string.Empty);
}