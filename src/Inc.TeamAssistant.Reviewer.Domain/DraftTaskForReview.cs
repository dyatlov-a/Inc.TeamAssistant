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
        long chatId,
        int messageId,
        string description,
        DateTimeOffset now,
        NextReviewerType strategy)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        
        Id = id;
        TeamId = teamId;
        OwnerId = ownerId;
        ChatId = chatId;
        MessageId = messageId;
        Description = description;
        Created = now;
        Strategy = strategy;
    }
    
    public DraftTaskForReview WithDescription(string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        
        Description = description;
        
        return this;
    }
    
    public DraftTaskForReview SetTargetPerson(long? personId)
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
            throw new TeamAssistantUserException(Messages.Connector_HasNoRights, personId);

        return this;
    }

    public NextReviewerType GetStrategy() => TargetPersonId.HasValue ? NextReviewerType.Target : Strategy;
}