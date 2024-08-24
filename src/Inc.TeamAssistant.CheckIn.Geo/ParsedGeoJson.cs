namespace Inc.TeamAssistant.CheckIn.Geo;

internal sealed class ParsedGeoJson
{
    public string Id { get; set; } = default!;
    public IDictionary<string, string> Properties { get; set; } = default!;
    public float[][] Geometry { get; set; } = default!;
}