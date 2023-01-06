using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Appraiser.Application.Contracts;

public sealed record LanguageContext(LanguageId LanguageId, string Command);