namespace Inc.TeamAssistant.CheckIn.Geo.Models;

internal sealed record PolygonGeometry(float[][][] Coordinates)
    : IGeometry
{
    public IEnumerable<float[]> GetCoordinates()
    {
        foreach (var item in Coordinates[0])
            yield return [item[0], item[1]];
    }
}