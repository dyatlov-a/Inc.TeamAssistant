namespace Inc.TeamAssistant.WebUI.Features.Logins;

public sealed record TelegramLoginViewModel(string LoginTelegramText, string BotUserName)
{
    public static readonly TelegramLoginViewModel Empty = new(string.Empty, string.Empty);
}