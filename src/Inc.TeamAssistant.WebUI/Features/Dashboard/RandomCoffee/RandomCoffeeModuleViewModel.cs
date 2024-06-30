using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.RandomCoffee;

public sealed record RandomCoffeeModuleViewModel(string RandomCoffeeHistoryWidgetTitle)
    : IViewModel<RandomCoffeeModuleViewModel>
{
    public static RandomCoffeeModuleViewModel Empty { get; } = new(string.Empty);
}