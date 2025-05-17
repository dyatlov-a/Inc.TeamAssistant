namespace Inc.TeamAssistant.Retro.Domain;

public static class RetroSessionStateRules
{
    public static readonly IReadOnlyCollection<RetroSessionState> Active =
    [
        RetroSessionState.Reviewing,
        RetroSessionState.Prioritizing,
        RetroSessionState.Discussing
    ];
}