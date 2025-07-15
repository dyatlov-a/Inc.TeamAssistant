namespace Inc.TeamAssistant.WebUI.Features.Rooms;

public static class RetroTypes
{
    public const string Hidden = "Hidden";
    public const string Closed = "Closed";
    public const string Opened = "Opened";
    
    public static readonly IReadOnlyCollection<string> All = [Hidden, Closed, Opened];
}