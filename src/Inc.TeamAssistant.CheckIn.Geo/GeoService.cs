using GeoTimeZone;
using Inc.TeamAssistant.CheckIn.Application.Contracts;
using Inc.TeamAssistant.CheckIn.Geo.Models;

namespace Inc.TeamAssistant.CheckIn.Geo;

internal sealed class GeoService : IGeoService
{
    private readonly Region[] _regions;

    public GeoService(RegionLoader regionLoader, GeoParser geoParser)
    {
        ArgumentNullException.ThrowIfNull(regionLoader);
        ArgumentNullException.ThrowIfNull(geoParser);
        
        _regions = geoParser.Parse(regionLoader.LoadFile()).ToArray();
    }
    
    public Region? FindCountry(double lat, double lng, params RegionType[] types)
    {
        ArgumentNullException.ThrowIfNull(types);
        
        var point = new Point((float)lng, (float)lat);
        var items = types.Any()
            ? _regions.Where(x => types.Any(y => y == x.Type))
            : _regions;

        foreach (var item in items)
            if (InPolygon(point, item.Polygon))
                return item;

        return null;
    }

    public TimeZoneInfo GetTimeZone(double lat, double lng)
    {
        var timeZoneId = TimeZoneLookup.GetTimeZone(lat, lng);
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Result);
    }
    
    private bool InPolygon(Point point, float[][] polygon)
    {
        ArgumentNullException.ThrowIfNull(point);
        ArgumentNullException.ThrowIfNull(polygon);
        
        var result = false;
        var i = 0;
        var j = 0;
        
        for (i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            if (polygon[i][1] > point.Lat != polygon[j][1] > point.Lat &&
                point.Lng < (polygon[j][0] - polygon[i][0]) * (point.Lat - polygon[i][1]) /
                (polygon[j][1] - polygon[i][1]) + polygon[i][0])
                result = !result;

        return result;
    }
}