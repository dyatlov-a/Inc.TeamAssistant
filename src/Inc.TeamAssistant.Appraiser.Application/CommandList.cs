namespace Inc.TeamAssistant.Appraiser.Application;

internal static class CommandList
{
    public const string AddStory = "/add";
    public const string Finish = "/finish?storyId=";
    public const string Revote = "/revote?storyId=";
    public const string AcceptEstimate = "/accept?value={0}&storyId=";
    public const string Set = "/set?value={0}&storyId=";
    public const string MoveToSp = "/move_to_sp";
    public const string MoveToTShirts = "/move_to_tshirts";
}