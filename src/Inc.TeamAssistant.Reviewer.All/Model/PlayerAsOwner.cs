using Inc.TeamAssistant.Appraiser.Primitives;

namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class PlayerAsOwner
{
    public Guid Id { get; private set; }
    public long LastReviewerId { get; private set; }
    public LanguageId LanguageId { get; private set; } = default!;
    public long UserId { get; private set; }

    private PlayerAsOwner()
    {
    }

    public PlayerAsOwner(Player player, long lastReviewerId)
        : this()
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));
        
        Id = player.Id;
        LanguageId = player.LanguageId;
        UserId = player.UserId;
        LastReviewerId = lastReviewerId;
    }
}