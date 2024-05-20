using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed record StagesPageViewModel(string Title, StagesState StagesState)
    : IViewModel<StagesPageViewModel>
{
    public static StagesPageViewModel Empty { get; } = new(string.Empty, StagesState.Empty);
}