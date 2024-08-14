namespace Inc.TeamAssistant.Gateway.Configs;

public static class CachePolicies
{
    public static TimeSpan CacheAbsoluteExpiration = TimeSpan.FromMinutes(30);
    
    public const int UserAvatarCacheDurationInSeconds = 60 * 60;
    public const int UserAvatarClientCacheDurationInSeconds = 60 * 15;
    
    public const string OpenGraphCachePolicyName = "open_graph";
    public const int OpenGraphCacheDurationInSeconds = 60 * 60 * 24;
}