using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed record DbPlayer(
    Guid Id,
    Guid TeamId,
    long PersonId,
    LanguageId PersonLanguageId,
    string PersonFirstName,
    string? PersonLastName,
    string? PersonUsername);