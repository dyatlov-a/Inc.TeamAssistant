using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed class TaskForReview
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
        Guid botId,
        Guid teamId,
        DateTimeOffset now,
        TimeSpan notificationInterval,
        NextReviewerType strategy,
        long ownerId,
        long chatId,
        string description)
        : this()
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

        Id = Guid.NewGuid();
        BotId = botId;
        Strategy = strategy;
        Created = now;
        TeamId = teamId;
        OwnerId = ownerId;
        ChatId = chatId;
        Description = description;
        State = TaskForReviewState.New;
        SetNextNotificationTime(now, notificationInterval);
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
                throw new ArgumentOutOfRangeException(nameof(messageType));
        }
    }

    public void SetNextNotificationTime(DateTimeOffset now, TimeSpan notificationInterval)
    {
        const int inProgressIndex = 2;
        var interval = State == TaskForReviewState.InProgress
            ? notificationInterval * inProgressIndex
            : notificationInterval;
        NextNotification = now.Add(interval);
    }

    public bool CanAccept() => TaskForReviewStateRules.ActiveStates.Contains(State);

    public void Accept(DateTimeOffset now)
    {
        AddReviewInterval(ReviewerId, now);
        
        AcceptDate = now;

        MoveToAccept();
    }
    
    public void MoveToAccept() => State = TaskForReviewState.Accept;

    public void Decline(DateTimeOffset now)
    {
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.OnCorrection;
        NextNotification = now;
    }

    public bool CanMoveToNextRound() => State == TaskForReviewState.OnCorrection;

    public void MoveToNextRound(DateTimeOffset now)
    {
        AddReviewInterval(OwnerId, now);
        
        State = TaskForReviewState.InProgress;
        NextNotification = now;
    }

    public bool CanMoveToInProgress() => State == TaskForReviewState.New;

    public void MoveToInProgress(TimeSpan notificationInterval, DateTimeOffset now)
    {
        AddReviewInterval(ReviewerId, now);
        
        State = TaskForReviewState.InProgress;
        SetNextNotificationTime(now, notificationInterval);
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
        
        State = TaskForReviewState.New;
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

        INextReviewerStrategy reviewerStrategy = Strategy switch
        {
            NextReviewerType.Random => new RandomReviewerStrategy(teammates, history),
            NextReviewerType.RoundRobin => new RoundRobinReviewerStrategy(teammates),
            _ => throw new TeamAssistantException($"Strategy {Strategy} was not supported.")
        };

        var excludedPersonIds = excludedPersonId.HasValue
            ? new[] { OwnerId, excludedPersonId.Value }
            : [OwnerId];
        SetReviewer(reviewerStrategy.Next(excludedPersonIds, lastReviewerId));

        return this;
    }
    
    private void SetReviewer(long reviewerId)
    {
        if (!OriginalReviewerId.HasValue && ReviewerId != 0)
            OriginalReviewerId = ReviewerId;
            
        ReviewerId = reviewerId;
    }
}