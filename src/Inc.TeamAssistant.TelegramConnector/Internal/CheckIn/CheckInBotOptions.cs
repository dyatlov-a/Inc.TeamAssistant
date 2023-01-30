namespace Inc.TeamAssistant.TelegramConnector.Internal.CheckIn;

internal sealed class CheckInBotOptions
{
    public string BaseUrl { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string ConnectToMapLinkTemplate { get; set; } = default!;
}