namespace Inc.TeamAssistant.WebUI.Features.Constructor;

public sealed record ConstructorPageViewModel(
    string Title,
    string SelectBotText,
    string LoginTelegramText,
    string BotUserName)
{
    public static readonly ConstructorPageViewModel Empty = new(string.Empty, string.Empty, string.Empty, string.Empty);
}