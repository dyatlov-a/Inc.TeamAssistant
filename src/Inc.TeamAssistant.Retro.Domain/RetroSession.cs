namespace Inc.TeamAssistant.Retro.Domain;

public sealed class RetroSession
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public RetroSessionState State { get; private set; }

    private RetroSession()
    {
    }

    public RetroSession(Guid id, Guid roomId, DateTimeOffset created)
        : this()
    {
        Id = id;
        RoomId = roomId;
        Created = created;
        State = RetroSessionState.Grouping;
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
}