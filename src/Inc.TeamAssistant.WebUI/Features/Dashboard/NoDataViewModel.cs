using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard;

public sealed record NoDataViewModel(string Text)
    : IViewModel<NoDataViewModel>
{
    public static string PersistentKey => Guid.NewGuid().ToString("N");
    public static NoDataViewModel Empty { get; } = new(string.Empty);
}