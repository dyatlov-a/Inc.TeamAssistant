using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Internal;

internal sealed record DbPerson(
    Guid PlayerId,
    long Id,
    LanguageId LanguageId,
    string FirstName,
    string? LastName,
    string? Username);