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
    public long? FirstReviewerId { get; private set; }
    public int? FirstReviewerMessageId { get; private set; }
    public IReadOnlyCollection<ReviewInterval> ReviewIntervals { get; private set; } = [];
    public IReadOnlyCollection<ReviewComment> Comments { get; private set; } = [];

    private TaskForReview()
    {
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
        
        RescheduleNotify(now, notificationIntervals);
        ChangeReviewer(reviewerId);
    }
    
    public bool CanReassign() => !OriginalReviewerId.HasValue && !FirstReviewerId.HasValue && ReviewerId != OwnerId;
    public bool CanMoveToInProgress() => State is TaskForReviewState.New or TaskForReviewState.FirstAccept;
    public bool CanMakeDecision() => TaskForReviewStateRules.ActiveStates.Contains(State);
    public bool CanMoveToNextRound() => State == TaskForReviewState.OnCorrection;
    public bool HasRightsForComments(long authorId)
        => new[] { OwnerId, ReviewerId, OriginalReviewerId, FirstReviewerId }.Any(i => i == authorId);
    public long? GetFirstRoundReviewerId() => FirstReviewerId.HasValue && FirstReviewerId.Value != ReviewerId
        ? FirstReviewerId.Value
        : null;

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

    public TaskForReview RescheduleNotify(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        NextNotification = now.Add(notificationIntervals.GetNotificationInterval(State));

        return this;
    }

    public TaskForReview AddComment(DateTimeOffset now, string comment, long authorId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(comment);
        
        Comments = Comments.Append(new ReviewComment(now, comment, authorId)).ToArray();

        return this;
    }

    public TaskForReview FinishRound(DateTimeOffset now, long? secondReviewerId = null)
    {
        AddReviewInterval(ReviewerId, now);

        var isSecondRound = FirstReviewerId.HasValue;
        FirstReviewerId ??= ReviewerId;
        FirstReviewerMessageId ??= ReviewerMessageId;

        if (!isSecondRound &&
            secondReviewerId.HasValue &&
            FirstReviewerId.Value != secondReviewerId.Value &&
            OwnerId != secondReviewerId.Value)
        {
            ChangeReviewer(secondReviewerId.Value);
            State = TaskForReviewState.FirstAccept;
        }
        else
        {
            AcceptDate = now;
            State = Comments.Any() ? TaskForReviewState.AcceptWithComments : TaskForReviewState.Accept;
        }

        return this;
    }

    public TaskForReview Decline(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.OnCorrection;
        NextNotification = now;

        var firstMoveToCorrection = ReviewIntervals.All(i => i.State != TaskForReviewState.OnCorrection);
        if (firstMoveToCorrection)
            RescheduleNotify(now, notificationIntervals);

        return this;
    }

    public TaskForReview MoveToNextRound(DateTimeOffset now)
    {
        AddReviewInterval(OwnerId, now);
        
        State = TaskForReviewState.InProgress;
        NextNotification = now;

        return this;
    }
    
    public TaskForReview MoveToInProgress(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.InProgress;
        RescheduleNotify(now, notificationIntervals);

        return this;
    }

    public TaskForReview Reassign(DateTimeOffset now, long reviewerId)
    {
        AddReviewInterval(ReviewerId, now);
        
        OriginalReviewerId ??= ReviewerId;
        NextNotification = now;
        
        ChangeReviewer(reviewerId);

        return this;
    }
    
    public MessageId ReviewerInMessage()
    {
        var hasReassign = OriginalReviewerId.HasValue && !FirstReviewerId.HasValue;
        var messageId = (hasReassign, Strategy) switch
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
            TaskForReviewState.New => GlobalResources.Icons.New,
            TaskForReviewState.FirstAccept => GlobalResources.Icons.FirstAccept,
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
    
    private void AddReviewInterval(long userId, DateTimeOffset end)
    {
        ReviewIntervals = ReviewIntervals.Append(new ReviewInterval(State, end, userId)).ToArray();
    }
    
    private void ChangeReviewer(long reviewerId)
    {
        ReviewerId = reviewerId;
        ReviewerMessageId = null;
    }
}