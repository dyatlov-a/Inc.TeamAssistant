namespace Inc.TeamAssistant.WebUI.Features.Retro;

public static class RetroStages
{
    public const string CollectingState = "Collecting";
    public const string GroupingState = "Grouping";
    public const string PrioritizingState = "Prioritizing";
    public const string DiscussingState = "Discussing";

    public static readonly IReadOnlyCollection<string> All =
    [
        CollectingState,
        GroupingState,
        PrioritizingState,
        DiscussingState
    ];
}