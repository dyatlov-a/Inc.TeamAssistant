using Inc.TeamAssistant.Connector.Model.Queries.GetBots;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record DashboardTeamSelectorViewModel(IReadOnlyCollection<BotDto> Bots)
    : IViewModel<DashboardTeamSelectorViewModel>
{
    public static DashboardTeamSelectorViewModel Empty { get; } = new(Array.Empty<BotDto>());
}