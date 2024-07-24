using Inc.TeamAssistant.Connector.Model.Queries.GetBots;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record DashboardPageViewModel(
    string BotWidgetTitle,
    IReadOnlyCollection<BotDto> Bots)
    : IViewModel<DashboardPageViewModel>
{
    public static DashboardPageViewModel Empty { get; } = new(
        string.Empty,
        Array.Empty<BotDto>());
}