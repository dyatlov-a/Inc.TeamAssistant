using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Main;

public sealed record MainPageViewModel(
    string RequestDemo,
    string RequestDemoSubject,
    string RequestDemoBody,
    string Header,
    string SubHeader,
    string ToolAppraiser,
    string ToolReviewer,
    string ToolRandomCoffee,
    string CreateBotLink,
    string NotSupportedMessage)
    : IViewModel<MainPageViewModel>
{
    public static MainPageViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}