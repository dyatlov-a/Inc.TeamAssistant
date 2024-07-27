using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Meta;

public sealed record MetaDataViewModel(
    string Description,
    string Keywords,
    string Author,
    string RobotsValue)
    : IViewModel<MetaDataViewModel>
{
    public static MetaDataViewModel Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}