using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Logins;

public sealed record TelegramLoginViewModel(string LoginTelegramText, string BotUserName)
    : IViewModel<TelegramLoginViewModel>
{
    public static TelegramLoginViewModel Empty { get; } = new(string.Empty, string.Empty);
}