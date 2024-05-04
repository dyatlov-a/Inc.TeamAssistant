using Inc.TeamAssistant.Constructor.Model.Queries.GetBotsByOwner;

namespace Inc.TeamAssistant.WebUI.Features.Constructor;

public sealed record BotSelectorViewModel(string AddBotLink, IReadOnlyCollection<BotDto> Bots)
{
    public static readonly BotSelectorViewModel Empty = new(string.Empty, Array.Empty<BotDto>());
}