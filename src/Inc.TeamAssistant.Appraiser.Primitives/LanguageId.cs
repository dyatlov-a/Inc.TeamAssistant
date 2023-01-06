namespace Inc.TeamAssistant.Appraiser.Primitives;

public sealed record LanguageId
{
    public string Value { get; }

    public LanguageId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

        Value = value;
    }
}