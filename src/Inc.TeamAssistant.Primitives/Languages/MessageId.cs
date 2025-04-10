namespace Inc.TeamAssistant.Primitives.Languages;

public sealed record MessageId
{
    public string Value { get; }

    public MessageId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
    }
}