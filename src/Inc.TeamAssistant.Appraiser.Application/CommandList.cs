namespace Inc.TeamAssistant.Appraiser.Application;

internal static class CommandList
{
    public const string AddStory = "/add";
    public const string Finish = "/finish?storyId=";
    public const string Revote = "/revote?storyId=";
    public const string AcceptEstimate = "/accept?value={0}&storyId=";
    public const string Set = "/set?value={0}&storyId=";
    public const string MoveToFibonacci = "/move_to_fibonacci";
    public const string MoveToTShirts = "/move_to_tshirts";
    public const string MoveToPowerOfTwo = "/move_to_power_of_two";
}