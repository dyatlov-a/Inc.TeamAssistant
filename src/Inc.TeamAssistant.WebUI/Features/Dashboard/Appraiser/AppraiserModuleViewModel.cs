using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public record AppraiserModuleViewModel(string SessionHistoryWidgetTitle)
    : IViewModel<AppraiserModuleViewModel>
{
    public static AppraiserModuleViewModel Empty { get; } = new(string.Empty);
}