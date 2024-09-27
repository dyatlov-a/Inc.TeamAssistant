using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Inc.TeamAssistant.CheckIn.Geo.Models;

namespace Inc.TeamAssistant.CheckIn.Geo;

internal sealed class GeometryResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(options);
        
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        
        if (jsonTypeInfo.Type == typeof(IGeometry))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "type",
                IgnoreUnrecognizedTypeDiscriminators = false,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(PolygonGeometry), "Polygon"),
                    new JsonDerivedType(typeof(MultiPolygonGeometry), "MultiPolygon"),
                }
            };
        }

        return jsonTypeInfo;
    }
}