using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed record Person(long Id, LanguageId LanguageId, string FirstName, string? LastName, string? Username)
{
    public bool HasUsername() => !string.IsNullOrWhiteSpace(Username);
    
    public string GetPersonLink() => HasUsername() ? $"@{Username}" : FirstName;

    public string GetFullName() => string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";

    public override string ToString() => GetFullName();
}