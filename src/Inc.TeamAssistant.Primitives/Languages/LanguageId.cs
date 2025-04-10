namespace Inc.TeamAssistant.Primitives.Languages;

public sealed record LanguageId
{
    public string Value { get; }

    public LanguageId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.ToLowerInvariant();
    }
}