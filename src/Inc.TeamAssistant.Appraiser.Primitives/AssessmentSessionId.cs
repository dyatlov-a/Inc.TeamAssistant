namespace Inc.TeamAssistant.Appraiser.Primitives;

public sealed record AssessmentSessionId
{
    public Guid Value { get; }

    public AssessmentSessionId(Guid value)
    {
        if (value == default)
            throw new ArgumentException("Value cannot be default.", nameof(value));

        Value = value;
    }
}