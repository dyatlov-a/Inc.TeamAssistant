namespace Inc.TeamAssistant.Appraiser.Application;

internal static class CommandList
{
    public const string AddStory = "/add";
    public const string Finish = "/finish?storyId=";
    public const string Revote = "/revote?storyId=";
    public const string AcceptEstimate = "/accept?storyId=";
    public const string Set = "/set?value={0}&storyId=";
}