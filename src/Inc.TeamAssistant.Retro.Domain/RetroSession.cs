using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroSession
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public RetroSessionState State { get; private set; }
    public long FacilitatorId { get; private set; }

    private RetroSession()
    {
    }

    public RetroSession(Guid id, Guid teamId, DateTimeOffset created, long facilitatorId)
        : this()
    {
        Id = id;
        TeamId = teamId;
        Created = created;
        State = RetroSessionState.Grouping;
        FacilitatorId = facilitatorId;
    }
    
    internal bool HasRights(long personId) => FacilitatorId == personId;

    public RetroSession EnsureRights(long personId)
    {
        if (!HasRights(personId))
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);
        
        return this;
    }
    
    public RetroSession MoveToNextState()
    {
        State = State switch
        {
            RetroSessionState.Grouping => RetroSessionState.Prioritizing,
            RetroSessionState.Prioritizing => RetroSessionState.Discussing,
            RetroSessionState.Discussing => RetroSessionState.Finished,
            _ => throw new InvalidOperationException($"Invalid state transition from {State}")
        };

        return this;
    }
    
    public RetroSession ChangeFacilitator(long personId)
    {
        FacilitatorId = personId;
        
        return this;
    }
}