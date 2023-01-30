namespace Inc.TeamAssistant.CheckIn.Primitives;

public sealed record MapId
{
    public Guid Value { get; }

    public MapId(Guid value)
    {
        if (value == default)
            throw new ArgumentException("Value cannot be default.", nameof(value));

        Value = value;
    }
}