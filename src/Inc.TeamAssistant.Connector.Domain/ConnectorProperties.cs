using System.Diagnostics.CodeAnalysis;

namespace Inc.TeamAssistant.Connector.Domain;

public static class ConnectorProperties
{
    public static readonly PropertyKey AccessToken = new("accessToken");
    public static readonly PropertyKey ProjectKey = new("projectKey");
    public static readonly PropertyKey ScrumMaster = new("scrumMaster");

    public static bool TryGetValue(
        this IReadOnlyDictionary<string, string> properties,
        PropertyKey propertyKey,
        [MaybeNullWhen(false)] out string value)
    {
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(propertyKey);

        return properties.TryGetValue(ScrumMaster.Key, out value);
    }
    
    public static string GetPropertyValueOrDefault(
        this IReadOnlyDictionary<string, string> properties,
        PropertyKey propertyKey,
        string defaultValue = "")
    {
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(propertyKey);
        ArgumentNullException.ThrowIfNull(defaultValue);
        
        return properties.GetValueOrDefault(propertyKey.Key, defaultValue);
    }
}