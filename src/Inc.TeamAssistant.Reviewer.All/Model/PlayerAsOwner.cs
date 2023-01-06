namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class PlayerAsOwner
{
    public Guid Id { get; private set; }
    public long LastReviewerId { get; private set; }

    private PlayerAsOwner()
    {
    }

    public PlayerAsOwner(Guid playerId, long lastReviewerId)
        : this()
    {
        Id = playerId;
        LastReviewerId = lastReviewerId;
    }
}