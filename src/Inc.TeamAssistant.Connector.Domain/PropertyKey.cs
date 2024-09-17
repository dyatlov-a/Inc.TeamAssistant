namespace Inc.TeamAssistant.Connector.Domain;

public sealed record PropertyKey
{
    internal string Key { get; }

    public PropertyKey(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
            
        Key = key;
    }
}