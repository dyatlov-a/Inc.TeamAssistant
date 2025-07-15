namespace Inc.TeamAssistant.WebUI.Features.Retro;

public static class ActionItemStages
{
    public const string New = "New";
    public const string Done = "Done";
    public const string Pinned = "Pinned";

    public static readonly IReadOnlyCollection<string> All = [New, Done, Pinned];
}