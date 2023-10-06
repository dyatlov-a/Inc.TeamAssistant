using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Users;

public sealed record PersonDto(long Id, LanguageId LanguageId, string FirstName, string? LastName, string? Username);