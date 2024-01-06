using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Person
{
    public long Id { get; private set; }
    public string Name { get; private set; } = default!;
    public LanguageId LanguageId { get; private set; } = default!;
    public string? Username { get; private set; }

    private Person()
    {
    }
    
    public Person(long id, string name, LanguageId languageId, string? username)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Id = id;
        Name = name;
        LanguageId = languageId ?? throw new ArgumentNullException(nameof(languageId));
        Username = username;
    }
}