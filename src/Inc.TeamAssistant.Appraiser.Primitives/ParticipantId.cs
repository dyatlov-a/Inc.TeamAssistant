namespace Inc.TeamAssistant.Appraiser.Primitives;

public sealed record ParticipantId
{
    public long Value { get; }

    public ParticipantId(long value)
    {
        if (value == default)
            throw new ArgumentException("Value cannot be default.", nameof(value));

        Value = value;
    }
}