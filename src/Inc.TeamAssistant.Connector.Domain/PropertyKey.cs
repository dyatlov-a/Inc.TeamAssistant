namespace Inc.TeamAssistant.Connector.Domain;

public sealed record PropertyKey
{
    internal string Key { get; }

    public PropertyKey(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
            
        Key = key;
    }
        
    public static readonly PropertyKey AccessToken = new("accessToken");
    public static readonly PropertyKey ProjectKey = new("projectKey");
    public static readonly PropertyKey ScrumMaster = new("scrumMaster");
}