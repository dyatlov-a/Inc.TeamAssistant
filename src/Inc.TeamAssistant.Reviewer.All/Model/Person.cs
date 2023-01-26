using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class Person
{
    public long Id { get; private set; }
    public LanguageId LanguageId { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string? LastName { get; private set; }
    public string? Username { get; private set; }

    private Person()
    {
    }

    public Person(long id, LanguageId languageId, string firstName, string? lastName, string? username)
    : this()
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(firstName));
        
        Id = id;
        LanguageId = languageId ?? throw new ArgumentNullException(nameof(languageId));
        FirstName = firstName;
        LastName = lastName;
        Username = username;
    }
    
    public bool HasUsername() => !string.IsNullOrWhiteSpace(Username);
    
    public string GetPersonLink() => HasUsername() ? $"@{Username}" : FirstName;

    public string GetFullName() => string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";

    public override string ToString() => GetFullName();
}