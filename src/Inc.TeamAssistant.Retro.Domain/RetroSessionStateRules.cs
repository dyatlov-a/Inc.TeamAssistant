namespace Inc.TeamAssistant.Retro.Domain;

public static class RetroSessionStateRules
{
    public static readonly IReadOnlyCollection<RetroSessionState> Active =
    [
        RetroSessionState.Grouping,
        RetroSessionState.Prioritizing,
        RetroSessionState.Discussing
    ];
}