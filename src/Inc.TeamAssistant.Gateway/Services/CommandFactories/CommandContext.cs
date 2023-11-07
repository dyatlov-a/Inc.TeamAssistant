using Inc.TeamAssistant.Appraiser.Primitives;
using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Gateway.Services.CommandFactories;

public sealed record CommandContext(
    string Cmd,
    long ChatId,
    ParticipantId UserId,
    string UserName,
    LanguageId LanguageId);