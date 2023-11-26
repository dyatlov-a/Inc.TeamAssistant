using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

public sealed record CommandContext(
    string Cmd,
    long ChatId,
    long UserId,
    string UserName,
    LanguageId LanguageId);