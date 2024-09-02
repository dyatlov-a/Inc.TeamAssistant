using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.CheckIn;

public sealed class MapWidgetFormModel
{
    public Guid? MapId { get; set; }
    public IReadOnlyCollection<MapDto> Maps { get; set; } = Array.Empty<MapDto>();
}