namespace Inc.TeamAssistant.Gateway;

public sealed class TelegramBotOptions
{
    public string Link { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public string ConnectToSessionLinkTemplate { get; set; } = default!;
	public string ConnectToDashboardLinkTemplate { get; set; } = default!;
    public TimeSpan CacheAbsoluteExpiration { get; set; }
}