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
{
    public static readonly RequestDemoViewModel Empty = new(
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