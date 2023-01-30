namespace Inc.TeamAssistant.CheckIn.Primitives;

public sealed record LocationOnMapId
{
    public Guid Value { get; }

    public LocationOnMapId(Guid value)
    {
        if (value == default)
            throw new ArgumentException("Value cannot be default.", nameof(value));

        Value = value;
    }
}