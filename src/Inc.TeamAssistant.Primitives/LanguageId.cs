namespace Inc.TeamAssistant.Primitives;

public sealed record LanguageId
{
    public string Value { get; }

    public LanguageId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

        Value = value.ToLowerInvariant();
    }

    public static LanguageId? Build(string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return null;

        var languageId = new LanguageId(languageCode);
        
        return LanguageSettings.LanguageIds.Contains(languageId) ? languageId : null;
    }
}