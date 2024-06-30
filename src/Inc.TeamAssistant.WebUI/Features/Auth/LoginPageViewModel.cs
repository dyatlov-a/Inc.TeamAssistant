using Inc.TeamAssistant.WebUI.Features.Common;

namespace Inc.TeamAssistant.WebUI.Features.Auth;

public sealed record LoginPageViewModel(
    bool HasLoginAsSuperUser,
    string LoginAsSuperUserText,
    string Title,
    string CreateBotText,
    string LoginTelegramText,
    string BotUserName)
    : IViewModel<LoginPageViewModel>
{
    public static LoginPageViewModel Empty { get; } = new(
        false,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty);
}