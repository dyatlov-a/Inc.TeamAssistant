using Inc.TeamAssistant.Primitives.Exceptions;

namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed class DraftTaskForReview
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public long OwnerId { get; private set; }
    public NextReviewerType Strategy { get; private set; }
    public long ChatId { get; private set; }
    public int MessageId { get; private set; }
    public string Description { get; private set; } = default!;
    public long? TargetPersonId { get; private set; }
    public int? PreviewMessageId { get; private set; }
    public DateTimeOffset Created { get; private set; }

    private DraftTaskForReview()
    {
    }

    public DraftTaskForReview(
        Guid id,
        Guid teamId,
        long ownerId,
        NextReviewerType strategy,
        long chatId,
        int messageId,
        string description,
        DateTimeOffset now)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(description));
        
        Id = id;
        TeamId = teamId;
        OwnerId = ownerId;
        Strategy = strategy;
        ChatId = chatId;
        MessageId = messageId;
        Description = description;
        Created = now;
    }
    
    public DraftTaskForReview WithDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameof(description));
        
        Description = description;
        
        return this;
    }
    
    public DraftTaskForReview WithTargetPerson(long personId)
    {
        TargetPersonId = personId;
        
        return this;
    }
    
    public DraftTaskForReview WithPreviewMessage(int messageId)
    {
        PreviewMessageId = messageId;
        
        return this;
    }

    public DraftTaskForReview CheckRights(long personId)
    {
        if (OwnerId != personId)
            throw new TeamAssistantException("User has not rights for action.");

        return this;
    }
}