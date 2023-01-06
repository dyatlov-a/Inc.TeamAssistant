using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Reviewer.All.Contracts;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.All.Services;

public sealed record CommandContext(
    ITelegramBotClient Client,
    ITranslateProvider TranslateProvider,
    int MessageId,
    long UserId,
    string UserName,
    string? UserLogin,
    LanguageId LanguageId,
    long ChatId,
    string Text);