namespace Inc.TeamAssistant.CheckIn.Geo.Models;

internal sealed record MultiPolygonGeometry(float[][][][] Coordinates)
    : IGeometry
{
    public IEnumerable<float[]> GetCoordinates()
    {
        foreach (var coordinate in Coordinates)
        foreach (var item in coordinate[0])
            yield return [item[0], item[1]];
    }
}