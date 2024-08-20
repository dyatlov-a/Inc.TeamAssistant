using Inc.TeamAssistant.CheckIn.Application.Contracts;

namespace Inc.TeamAssistant.CheckIn.GeoCountry;

internal sealed class ReverseLookup : IReverseLookup
{
    private readonly GeoJsonParser _geoJsonParser;
    private readonly Region[] _regions;

    public ReverseLookup(FileLoader fileLoader, GeoJsonParser geoJsonParser)
    {
        _geoJsonParser = geoJsonParser;
        _regions = ParseInput(fileLoader.LoadFile()).ToArray();
    }

    private bool InPolygon(float[] point, float[][] polygon)
    {
        var nvert = polygon.Length;
        var c = false;
        var i = 0;
        var j = 0;
        for (i = 0, j = nvert - 1; i < nvert; j = i++)
        {
            if (polygon[i][1] > point[1] != (polygon[j][1] > point[1]) &&
                point[0] < (polygon[j][0] - polygon[i][0]) * (point[1] - polygon[i][1]) /
                (polygon[j][1] - polygon[i][1]) + polygon[i][0])
            {
                c = !c;
            }
        }

        return c;
    }
    
    public Region? Lookup(float lat, float lng, params RegionType[] types)
    {
        var coords = new[] { lng, lat };
        var subset = types.Any()
            ? _regions.Where(x => types.Any(y => y == x.Type))
            : _regions;

        foreach (var country in subset)
        {
            if (InPolygon(coords, country.Polygon))
            {
                return country;
            }
        }

        return null;
    }

    private IEnumerable<Region> ParseInput(IEnumerable<string> geojson)
    {
        foreach (var line in geojson)
        {
            foreach (var polygon in _geoJsonParser.Convert(line))
            {
                yield return new Region(
                    polygon.Properties["name"],
                    polygon.Id,
                    polygon.Properties["type"] == "country" ? RegionType.Country : RegionType.Ocean,
                    polygon.Geometry);
            }
        }
    }
}