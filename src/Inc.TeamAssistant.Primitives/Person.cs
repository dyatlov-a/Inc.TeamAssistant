namespace Inc.TeamAssistant.Primitives;

public sealed class Person
{
    public long Id { get; private set; }
    public string Name { get; private set; } = default!;
    public LanguageId? LanguageId { get; private set; }
    public string? Username { get; private set; }

    private Person()
    {
    }
    
    public Person(long id, string name, string? languageCode, string? username)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        
        Id = id;
        Name = name;
        LanguageId = LanguageId.Build(languageCode);
        Username = username;
    }

    public string DisplayName => string.IsNullOrWhiteSpace(Username) ? Name : $"{Name} ({Username})";

    public bool IsEquivalent(Person otherPerson)
    {
        if (otherPerson is null)
            throw new ArgumentNullException(nameof(otherPerson));
        
        if (ReferenceEquals(this, otherPerson))
            return true;

        if (Id != otherPerson.Id)
            return false;
        if (Name != otherPerson.Name)
            return false;
        if (Username != otherPerson.Username)
            return false;
        if (LanguageId != otherPerson.LanguageId)
            return false;

        return true;
    }

    public LanguageId GetLanguageId() => LanguageId ?? LanguageSettings.DefaultLanguageId;
}