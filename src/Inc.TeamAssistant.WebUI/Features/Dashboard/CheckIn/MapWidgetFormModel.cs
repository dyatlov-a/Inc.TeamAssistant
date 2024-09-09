using Inc.TeamAssistant.CheckIn.Model.Queries.GetMaps;

namespace Inc.TeamAssistant.WebUI.Features.Dashboard.CheckIn;

public sealed class MapWidgetFormModel
{
    public Guid? MapId { get; set; }

    private readonly List<MapDto> _maps = new();
    public IReadOnlyCollection<MapDto> Maps => _maps;

    public MapWidgetFormModel Apply(GetMapsResult maps)
    {
        ArgumentNullException.ThrowIfNull(maps);
        
        _maps.Clear();
        _maps.AddRange(maps.Items);
        
        MapId = maps.Items.FirstOrDefault()?.Id;

        return this;
    }
}