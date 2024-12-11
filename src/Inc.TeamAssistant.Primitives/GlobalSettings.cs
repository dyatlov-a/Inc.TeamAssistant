namespace Inc.TeamAssistant.Primitives;

public static class GlobalSettings
{
    public static readonly string CanonicalName = "teamassist.bot";

    public static readonly IReadOnlyCollection<string> LinksPrefix = ["http://", "https://"];
    
    public static readonly TimeSpan NotificationsDelay = TimeSpan.FromMinutes(1);

    public static readonly string LinkForConnectTemplate = "https://t.me/{0}?start={1}";
    
    public static readonly TimeSpan MinLoadingDelay = TimeSpan.FromMilliseconds(700);
    
    public static string TimeFormat = @"d\.hh\:mm\:ss";
}