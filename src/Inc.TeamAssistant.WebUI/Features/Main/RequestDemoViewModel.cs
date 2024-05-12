using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Main;

public sealed record RequestDemoViewModel(
    string RequestDemo,
    string RequestDemoSubject,
    string RequestDemoBody,
    string Header,
    string SubHeader,
    string ToolAppraiser,
    string ToolReviewer,
    string ToolRandomCoffee,
    string CreateBotLink)
    : IViewModel<RequestDemoViewModel>
{
    public static RequestDemoViewModel Empty { get; } = new(
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