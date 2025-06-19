namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroAssessment
{
    public Guid RetroSessionId { get; private set; }
    public long PersonId { get; private set; }
    public int Value { get; private set; }

    private RetroAssessment()
    {
    }

    public RetroAssessment(Guid retroSessionId, long personId)
        : this()
    {
        RetroSessionId = retroSessionId;
        PersonId = personId;
    }
    
    public RetroAssessment ChangeValue(int value)
    {
        Value = value;

        return this;
    }
}