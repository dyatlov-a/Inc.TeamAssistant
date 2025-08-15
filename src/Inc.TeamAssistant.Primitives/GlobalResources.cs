namespace Inc.TeamAssistant.Primitives;

public static class GlobalResources
{
    public static class Settings
    {
        public static readonly string CanonicalName = "easyteam.space";
        public static readonly IReadOnlyCollection<string> LinksPrefix = ["http://", "https://"];
        public static readonly TimeSpan NotificationsDelay = TimeSpan.FromMinutes(1);
        public static readonly string LinkForConnectTemplate = "https://t.me/{0}?start={1}";
        public static readonly TimeSpan MinLoadingDelay = TimeSpan.FromMilliseconds(700);
        public static readonly string OptionParameterName = "&option=";
    }
    
    public static class PersonStore
    {
        public static readonly TimeSpan ClearDelay = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan IdleConnectionLifetime = TimeSpan.FromHours(8);
    }
    
    public static class Icons
    {
        public static readonly string TrendUp = "👍";
        public static readonly string TrendDown = "👎";
        public static readonly string Alert = "❗";
        public static readonly string New = "⏳";
        public static readonly string FirstAccept = "⌛";
        public static readonly string InProgress = "🤩";
        public static readonly string OnCorrection = "😱";
        public static readonly string Accept = "🤝";
        public static readonly string AcceptWithComments = "🙏";
        public static readonly string Comment = "💬";
        public static readonly string Start = "⭐";
        public static readonly string Ok = "👌";
    }
}