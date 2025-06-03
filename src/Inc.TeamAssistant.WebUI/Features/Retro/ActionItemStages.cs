namespace Inc.TeamAssistant.WebUI.Features.Retro;

public static class ActionItemStages
{
    public const string NewState = "New";
    public const string DoneState = "Done";

    public static readonly IReadOnlyCollection<string> All = [NewState, DoneState];
}