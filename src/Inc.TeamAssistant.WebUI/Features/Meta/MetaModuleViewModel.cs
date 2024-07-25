using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record MetaModuleViewModel(string Title)
    : IViewModel<MetaModuleViewModel>
{
    public static MetaModuleViewModel Empty { get; } = new(string.Empty);
}