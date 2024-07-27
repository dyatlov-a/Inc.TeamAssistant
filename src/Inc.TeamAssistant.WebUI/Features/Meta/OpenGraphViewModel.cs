using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record OpenGraphViewModel(
    string LanguageId,
    string Title,
    string Description,
    string Image,
    string ImageText)
    : IViewModel<OpenGraphViewModel>
{
    public static OpenGraphViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}