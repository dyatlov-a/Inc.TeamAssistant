namespace Inc.TeamAssistant.CheckIn.Geo.Models;

internal interface IGeometry
{
    IEnumerable<float[]> GetCoordinates();
}