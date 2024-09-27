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
    public bool HasConcreteReviewer { get; private set; }
    public long? OriginalReviewerId { get; private set; }
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
        long chatId)
        : this()
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(notificationIntervals);

        Id = id;
        BotId = botId;
        Strategy = draft.Strategy;
        Created = now;
        TeamId = draft.TeamId;
        OwnerId = draft.OwnerId;
        ChatId = chatId;
        Description = draft.Description;
        State = TaskForReviewState.New;
        
        SetNextNotificationTime(now, notificationIntervals);
    }

    public void AttachMessage(MessageType messageType, int messageId)
    {
        switch (messageType)
        {
            case MessageType.Shared:
                MessageId = messageId;
                break;
            case MessageType.Reviewer:
                ReviewerMessageId = messageId;
                break;
            case MessageType.Owner:
                OwnerMessageId = messageId;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(messageType), messageType, "State out of range.");
        }
    }

    public void SetNextNotificationTime(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        NextNotification = now.Add(notificationIntervals.GetNotificationInterval(State));
    }

    public bool CanAccept() => TaskForReviewStateRules.ActiveStates.Contains(State);

    public void Accept(DateTimeOffset now)
    {
        AddReviewInterval(ReviewerId, now);
        
        AcceptDate = now;

        MoveToAccept();
    }
    
    public void MoveToAccept() => State = TaskForReviewState.Accept;

    public void Decline(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.OnCorrection;
        NextNotification = now;

        if (ReviewIntervals.All(i => i.State != TaskForReviewState.OnCorrection))
            SetNextNotificationTime(now, notificationIntervals);
    }

    public bool CanMoveToNextRound() => State == TaskForReviewState.OnCorrection;

    public void MoveToNextRound(DateTimeOffset now)
    {
        AddReviewInterval(OwnerId, now);
        
        State = TaskForReviewState.InProgress;
        NextNotification = now;
    }

    public bool CanMoveToInProgress() => State == TaskForReviewState.New;

    public void MoveToInProgress(DateTimeOffset now, NotificationIntervals notificationIntervals)
    {
        ArgumentNullException.ThrowIfNull(notificationIntervals);
        
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.InProgress;
        SetNextNotificationTime(now, notificationIntervals);
    }

    private void AddReviewInterval(long userId, DateTimeOffset end)
    {
        ReviewIntervals = ReviewIntervals.Append(new ReviewInterval(State, end, userId)).ToArray();
    }

    public TaskForReview SetConcreteReviewer(long reviewerId)
    {
        SetReviewer(reviewerId);
        HasConcreteReviewer = true;
        
        return this;
    }

    public TaskForReview Reassign(
        DateTimeOffset now,
        IReadOnlyCollection<long> teammates,
        IReadOnlyDictionary<long, int> history,
        long excludedPersonId,
        long? lastReviewerId)
    {
        ArgumentNullException.ThrowIfNull(teammates);
        ArgumentNullException.ThrowIfNull(history);
        
        AddReviewInterval(ReviewerId, now);
        
        NextNotification = now;
        ReviewerMessageId = null;

        return DetectReviewer(teammates, history, lastReviewerId, excludedPersonId);
    }

    public TaskForReview DetectReviewer(
        IReadOnlyCollection<long> teammates,
        IReadOnlyDictionary<long, int> history,
        long? lastReviewerId,
        long? excludedPersonId = null)
    {
        ArgumentNullException.ThrowIfNull(teammates);
        ArgumentNullException.ThrowIfNull(history);

        var excludedPersonIds = excludedPersonId.HasValue
            ? new[] { OwnerId, excludedPersonId.Value }
            : [OwnerId];
        var reviewerStrategy = NextReviewerStrategyFactory.Create(Strategy, teammates, history);
        
        SetReviewer(reviewerStrategy.Next(excludedPersonIds, lastReviewerId));

        return this;
    }

    public int? GetAttempts()
    {
        var corrections = ReviewIntervals.Count(i => i.State == TaskForReviewState.OnCorrection);
        return corrections == 0 ? null : corrections + 1;
    }

    public TimeSpan GetTotalTime(DateTimeOffset now) => now - Created;
    
    private void SetReviewer(long reviewerId)
    {
        if (!OriginalReviewerId.HasValue && ReviewerId != 0)
            OriginalReviewerId = ReviewerId;
            
        ReviewerId = reviewerId;
    }
}