namespace Inc.TeamAssistant.Reviewer.Application;

internal static class CommandList
{
    public const string MoveToInProgress = "/in_progress?storyId=";
    public const string Accept = "/approve?storyId=";
    public const string Decline = "/decline?storyId=";
    public const string MoveToNextRound = "/next_round?storyId=";
}