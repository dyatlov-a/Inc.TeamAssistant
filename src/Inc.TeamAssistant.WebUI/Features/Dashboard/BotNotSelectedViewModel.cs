using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record BotNotSelectedViewModel(string SelectTeam, string CreateBot)
    : IViewModel<BotNotSelectedViewModel>
{
    public static BotNotSelectedViewModel Empty { get; } = new(
        string.Empty,
        string.Empty);
}