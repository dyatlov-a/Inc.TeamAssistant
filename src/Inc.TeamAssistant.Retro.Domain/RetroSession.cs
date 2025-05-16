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
        State = RetroSessionState.Reviewing;
        FacilitatorId = facilitatorId;
    }

    public bool HasRights(long personId) => FacilitatorId == personId;
}