using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Inc.TeamAssistant.CheckIn.Geo;

internal sealed class GeoJsonParser
{
    public IEnumerable<ParsedGeoJson> Convert(string json)
    {
        return Convert(JsonConvert.DeserializeObject<dynamic>(json));
    }

    private IEnumerable<ParsedGeoJson> Convert(dynamic json)
    {
        foreach (var coordinates in ToPoints(json.geometry))
        {
            yield return new ParsedGeoJson
            {
                Id = json.id,
                Geometry = coordinates,
                Properties = ((IEnumerable<KeyValuePair<string, JToken>>)json.properties).ToDictionary(
                    k => k.Key.ToLower(),
                    v => v.Value.ToString())
            };
        }
    }

    private IEnumerable<float[][]> ToPoints(dynamic geometry)
    {
        if (null == geometry) yield break;
        switch ((string)geometry.type)
        {
            case "Polygon":
                yield return ((IEnumerable<float[]>)ParseCoordinates(geometry.coordinates[0])).ToArray();
                break;
            case "MultiPolygon":
                foreach (dynamic item in geometry.coordinates)
                {
                    yield return ((IEnumerable<float[]>)ParseCoordinates(item[0])).ToArray();
                }
                break;
            default:
                throw new NotImplementedException((string)geometry.type);
        }
    }

    private IEnumerable<float[]> ParseCoordinates(dynamic coordinates)
    {
        foreach (var coordinate in coordinates)
        {
            yield return [(float)coordinate[0], (float)coordinate[1]];
        }
    }
}