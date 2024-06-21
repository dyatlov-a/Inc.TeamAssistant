using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;
using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.CheckIn;

public sealed record MapWidgetViewModel(
    string MoveToMapTitle,
    IReadOnlyCollection<MapDto> Maps)
    : IViewModel<MapWidgetViewModel>
{
    public static MapWidgetViewModel Empty { get; } = new(string.Empty, Array.Empty<MapDto>());
}