using System.Text.Json;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Geo.Models;

namespace Inc.TeamAssistant.CheckIn.Geo;

internal sealed class GeoParser
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = new GeometryResolver()
    };
    
    public IEnumerable<Region> Parse(IEnumerable<string> json)
    {
        ArgumentNullException.ThrowIfNull(json);
        
        foreach (var line in json)
        {
            var item = JsonSerializer.Deserialize<GeoItem>(line, JsonSerializerOptions);

            if (item is not null)
                yield return new Region(
                    item.Properties.Name,
                    item.Id,
                    item.Properties.Type == "country" ? RegionType.Country : RegionType.Ocean,
                    item.Geometry.GetCoordinates().ToArray());
        }
    }
}