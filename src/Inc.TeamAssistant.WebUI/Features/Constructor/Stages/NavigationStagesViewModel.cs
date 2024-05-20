using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Constructor.Stages;

public sealed record NavigationStagesViewModel(IReadOnlyDictionary<Stage, string> StagesLookup)
    : IViewModel<NavigationStagesViewModel>
{
    public static NavigationStagesViewModel Empty { get; } = new(new Dictionary<Stage, string>());
}