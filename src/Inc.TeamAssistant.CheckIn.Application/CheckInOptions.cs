namespace Inc.TeamAssistant.CheckIn.Application;

public sealed class CheckInOptions
{
    public string AccessToken { get; set; } = default!;
    public string ConnectToMapLinkTemplate { get; set; } = default!;
}