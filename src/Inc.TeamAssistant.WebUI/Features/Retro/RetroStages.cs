namespace Inc.TeamAssistant.WebUI.Features.Retro;

public static class RetroStages
{
    public const string Collecting = "Collecting";
    public const string Grouping = "Grouping";
    public const string Prioritizing = "Prioritizing";
    public const string Discussing = "Discussing";
    public const string Finished = "Finished";

    public static readonly IReadOnlyCollection<string> Stages = [Collecting, Grouping, Prioritizing, Discussing];
}