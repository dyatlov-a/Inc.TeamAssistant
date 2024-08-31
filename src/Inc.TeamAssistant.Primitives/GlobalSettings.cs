namespace Inc.TeamAssistant.Primitives;

public static class GlobalSettings
{
    public static readonly string CanonicalName = "teamassist.bot";
    
    public static readonly string RequestDemoEmail = "dyatlovall@gmail.com";

    public static readonly IReadOnlyCollection<string> LinksPrefix = new[] { "http://", "https://" };
    
    public static readonly TimeSpan NotificationsDelay = TimeSpan.FromMinutes(1);

    public static readonly string LinkForConnectTemplate = "https://t.me/{0}?start={1}";
}