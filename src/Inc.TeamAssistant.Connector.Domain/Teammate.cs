namespace Inc.TeamAssistant.Connector.Domain;

public sealed class Teammate
{
    public Guid TeamId { get; private set; }
    public long PersonId { get; private set; }
    public DateTimeOffset? LeaveUntil { get; private set; }
    public bool CanFinalize { get; private set; }

    public Teammate ChangeLeaveUntil(DateTimeOffset? value)
    {
        LeaveUntil = value;
        
        return this;
    }
    
    public Teammate ChangeCanFinalize(bool value)
    {
        CanFinalize = value;

        return this;
    }
}