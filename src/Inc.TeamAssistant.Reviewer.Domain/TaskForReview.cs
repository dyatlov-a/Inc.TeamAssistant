using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed class TaskForReview : ITaskForReviewStats
{
    public Guid Id { get; private set; }
    public Guid BotId { get; private set; }
    public NextReviewerType Strategy { get; private set; }
    public Guid TeamId { get; private set; }
    public long OwnerId { get; private set; }
    public int? OwnerMessageId { get; private set; }
    public long ReviewerId { get; private set; }
    public int? ReviewerMessageId { get; private set; }
    public long ChatId { get; private set; }
    public string Description { get; private set; } = default!;
    public TaskForReviewState State { get; private set; }
    public DateTimeOffset Created { get; private set; }
    public DateTimeOffset NextNotification { get; private set; }
    public DateTimeOffset? AcceptDate { get; private set; }
    public int? MessageId { get; private set; }
    public long? OriginalReviewerId { get; private set; }
    public int? OriginalReviewerMessageId { get; private set; }
    public IReadOnlyCollection<ReviewInterval> ReviewIntervals { get; private set; }

    private TaskForReview()
    {
        ReviewIntervals = new List<ReviewInterval>();
    }

    public TaskForReview(
        Guid id,
        DraftTaskForReview draft,
        Guid botId,
        DateTimeOffset now,
        NotificationIntervals notificationIntervals,
        long chatId,
        long reviewerId)
        : this()
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(notificationIntervals);

        Id = id;
        BotId = botId;
        Strategy = draft.GetStrategy();
        Created = now;
        TeamId = draft.TeamId;
        OwnerId = draft.OwnerId;
        ChatId = chatId;
        Description = draft.Description;
        State = TaskForReviewState.New;
        
        SetNextNotificationTime(now, notificationIntervals);
        SetReviewer(reviewerId);
    }

    public TaskForReview AttachMessage(MessageType messageType, int messageId)
    {
        switch (messageType)
        {
            case MessageType.Shared:
                MessageId = messageId;
                break;
            case MessageType.Reviewer:
                OriginalReviewerMessageId ??= messageId;
                ReviewerMessageId = messageId;
                break;
            case MessageType.Owner:
                OwnerMessageId = messageId;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(messageType), messageType, "State out of range.");
        }

        return this;
    }

    public TaskForReview SetNextNotificationTime(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        NextNotification = now.Add(notificationIntervals.GetNotificationInterval(State));

        return this;
    }

    public bool CanAccept() => TaskForReviewStateRules.ActiveStates.Contains(State);

    public TaskForReview Accept(DateTimeOffset now, bool hasComments)
    {
        AddReviewInterval(ReviewerId, now);
        
        AcceptDate = now;
        State = hasComments
            ? TaskForReviewState.AcceptWithComments
            : TaskForReviewState.Accept;

        return this;
    }

    public TaskForReview Decline(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.OnCorrection;
        NextNotification = now;

        if (ReviewIntervals.All(i => i.State != TaskForReviewState.OnCorrection))
            SetNextNotificationTime(now, notificationIntervals);

        return this;
    }

    public bool CanMoveToNextRound() => State == TaskForReviewState.OnCorrection;

    public TaskForReview MoveToNextRound(DateTimeOffset now)
    {
        AddReviewInterval(OwnerId, now);
        
        State = TaskForReviewState.InProgress;
        NextNotification = now;

        return this;
    }

    public bool CanMoveToInProgress() => State == TaskForReviewState.New;

    public TaskForReview MoveToInProgress(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.InProgress;
        SetNextNotificationTime(now, notificationIntervals);

        return this;
    }

    private void AddReviewInterval(long userId, DateTimeOffset end)
    {
        ReviewIntervals = ReviewIntervals.Append(new ReviewInterval(State, end, userId)).ToArray();
    }
    
    public bool HasReassign() => OriginalReviewerId.HasValue && ReviewerId != OriginalReviewerId.Value;

    public TaskForReview Reassign(DateTimeOffset now, long reviewerId)
    {
        AddReviewInterval(ReviewerId, now);
        SetReviewer(reviewerId);
        
        NextNotification = now;

        return this;
    }
    
    public MessageId ReviewerInMessage()
    {
        var messageId = (HasReassign(), Strategy) switch
        {
            (true, _) => Messages.Reviewer_TargetReassigned,
            (_, NextReviewerType.Target) => Messages.Reviewer_TargetManually,
            (_, _) => Messages.Reviewer_TargetAutomatically
        };

        return messageId;
    }
    
    public string AsIcon()
    {
        return State switch
        {
            TaskForReviewState.New => GlobalResources.Icons.Waiting,
            TaskForReviewState.InProgress => GlobalResources.Icons.InProgress,
            TaskForReviewState.OnCorrection => GlobalResources.Icons.OnCorrection,
            TaskForReviewState.Accept => GlobalResources.Icons.Accept,
            TaskForReviewState.AcceptWithComments => GlobalResources.Icons.AcceptWithComments,
            _ => throw new ArgumentOutOfRangeException(nameof(State), State, "State out of range.")
        };
    }

    public int? GetAttempts()
    {
        var corrections = ReviewIntervals.Count(i => i.State == TaskForReviewState.OnCorrection);
        return corrections == 0 ? null : corrections + 1;
    }

    public TimeSpan GetTotalTime(DateTimeOffset now) => now - Created;
    
    private void SetReviewer(long reviewerId)
    {
        OriginalReviewerId ??= reviewerId;
        ReviewerId = reviewerId;
        
        ReviewerMessageId = null;
    }
}