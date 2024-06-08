using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor;

public sealed record BotSelectorViewModel(
    string NewBot,
    string AddBot,
    string Edit,
    string Remove,
    string RemoveConfirmationTemplate,
    string MoveToStats,
    IReadOnlyCollection<BotDto> Bots)
    : IViewModel<BotSelectorViewModel>
{
    public static BotSelectorViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        Array.Empty<BotDto>());
}