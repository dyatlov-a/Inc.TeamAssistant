using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record MetaViewModel(string Title, string Description, string Keywords, string Author)
    : IViewModel<MetaViewModel>
{
    public static MetaViewModel Empty { get; } = new(string.Empty, string.Empty, string.Empty, string.Empty);
}