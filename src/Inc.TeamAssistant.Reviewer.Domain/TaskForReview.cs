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
    public long ReviewerId { get; private set; }
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
        NextNotification = now;
    }

    public void AttachMessage(int messageId) => MessageId = messageId;

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
        AddReviewInterval(ReviewIntervalType.Review, now);
        
        AcceptDate = now;

        MoveToArchive();
    }
    
    public void MoveToArchive() => State = TaskForReviewState.IsArchived;

    public void Decline(DateTimeOffset now)
    {
        AddReviewInterval(ReviewIntervalType.Review, now);
        
        State = TaskForReviewState.OnCorrection;
        NextNotification = now;
    }

    public bool CanMoveToNextRound() => State == TaskForReviewState.OnCorrection;

    public void MoveToNextRound(DateTimeOffset now)
    {
        AddReviewInterval(ReviewIntervalType.Correction, now);
        
        State = TaskForReviewState.New;
        NextNotification = now;
    }

    public bool CanMoveToInProgress() => State == TaskForReviewState.New;

    public void MoveToInProgress(TimeSpan notificationInterval, DateTimeOffset now)
    {
        AddReviewInterval(ReviewIntervalType.FirstTouch, now);
        
        State = TaskForReviewState.InProgress;
        SetNextNotificationTime(now, notificationInterval);
    }

    private void AddReviewInterval(ReviewIntervalType type, DateTimeOffset end)
    {
        var start = ReviewIntervals.MaxBy(i => i.End)?.End ?? Created;
        
        ReviewIntervals = ReviewIntervals
            .Append(new ReviewInterval(type, start, end))
            .ToArray();
    }

    public TaskForReview SetConcreteReviewer(long reviewerId)
    {
        SetReviewer(reviewerId);
        HasConcreteReviewer = true;
        
        return this;
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