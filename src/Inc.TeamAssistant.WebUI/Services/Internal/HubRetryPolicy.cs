namespace Inc.TeamAssistant.WebUI.Services.Internal;

internal static class HubRetryPolicy
{
    public static readonly TimeSpan[] Default =
    [
        TimeSpan.Zero,
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    ];
}