using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.Appraiser;

public sealed record AppraiserIntegrationWidgetViewModel(
    string AccessTokenLabel,
    string ProjectKeyLabel,
    string ScrumMasterLabel,
    string SaveButton,
    string DisableButton,
    string EnableButton,
    string ConfirmText)
    : IViewModel<AppraiserIntegrationWidgetViewModel>
{
    public static AppraiserIntegrationWidgetViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}