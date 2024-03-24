namespace Inc.TeamAssistant.Primitives.Languages;

public sealed record MessageId
{
    public string Value { get; }

    public MessageId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));

        Value = value;
    }
}