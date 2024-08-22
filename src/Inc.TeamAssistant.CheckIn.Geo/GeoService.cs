using GeoTimeZone;
using Inc.TeamAssistant.CheckIn.Application.Contracts;

namespace Inc.TeamAssistant.CheckIn.Geo;

internal sealed class GeoService : IGeoService
{
    private readonly GeoJsonParser _geoJsonParser;
    private readonly Region[] _regions;

    public GeoService(RegionLoader regionLoader, GeoJsonParser geoJsonParser)
    {
        _geoJsonParser = geoJsonParser;
        _regions = ParseInput(regionLoader.LoadFile()).ToArray();
    }
    
    public Region? FindCountry(double lat, double lng, params RegionType[] types)
    {
        var coords = new[] { (float)lng, (float)lat };
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

    public TimeZoneInfo GetTimeZone(double lat, double lng)
    {
        var timeZoneId = TimeZoneLookup.GetTimeZone(lat, lng);
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Result);
    }
    
    private bool InPolygon(float[] point, float[][] polygon)
    {
        var polygonLength = polygon.Length;
        var c = false;
        var i = 0;
        var j = 0;
        for (i = 0, j = polygonLength - 1; i < polygonLength; j = i++)
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