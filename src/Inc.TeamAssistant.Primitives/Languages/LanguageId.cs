namespace Inc.TeamAssistant.Primitives.Languages;

public sealed record LanguageId
{
    public string Value { get; }

    public LanguageId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

        Value = value.ToLowerInvariant();
    }
}