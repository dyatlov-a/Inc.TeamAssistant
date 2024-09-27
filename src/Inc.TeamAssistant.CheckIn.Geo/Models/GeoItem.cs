namespace Inc.TeamAssistant.CheckIn.Geo.Models;

internal sealed record GeoItem(
    string Id,
    GeoProperties Properties,
    IGeometry Geometry);