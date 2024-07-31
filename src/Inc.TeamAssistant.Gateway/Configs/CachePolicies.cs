namespace Inc.TeamAssistant.Gateway.Configs;

public static class CachePolicies
{
    public const string UserAvatarCachePolicy = "user_avatar";
    public const int UserAvatarCacheDurationInSeconds = 60 * 60;
    public const string OpenGraphCachePolicy = "open_graph";
    public const int OpenGraphCacheDurationInSeconds = 60 * 60 * 24;
}