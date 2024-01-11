namespace Inc.TeamAssistant.Reviewer.Domain;

public sealed class TaskForReview
{
    public Guid Id { get; private set; }
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

    private TaskForReview()
    {
    }

    public TaskForReview(Guid teamId, long ownerId, long reviewerId, long chatId, string description)
        : this()
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

        Id = Guid.NewGuid();
        Created = DateTimeOffset.UtcNow;
        TeamId = teamId;
        OwnerId = ownerId;
        ReviewerId = reviewerId;
        ChatId = chatId;
        Description = description;
        State = TaskForReviewState.New;
        NextNotification = DateTimeOffset.UtcNow;
    }

    public void AttachMessage(int messageId) => MessageId = messageId;

    public void SetNextNotificationTime(TimeSpan notificationInterval)
    {
        const int inProgressIndex = 2;
        var interval = State == TaskForReviewState.InProgress
            ? notificationInterval * inProgressIndex
            : notificationInterval;
        NextNotification = DateTimeOffset.UtcNow.Add(interval);
    }

    public bool CanAccept() => State == TaskForReviewState.New || State == TaskForReviewState.InProgress;

    public void Accept()
    {
        State = TaskForReviewState.IsArchived;
        AcceptDate = DateTimeOffset.UtcNow;
    }

    public bool CanDecline() => State == TaskForReviewState.New || State == TaskForReviewState.InProgress;

    public void Decline()
    {
        State = TaskForReviewState.OnCorrection;
        NextNotification = DateTimeOffset.UtcNow;
    }

    public bool CanMoveToNextRound() => State == TaskForReviewState.OnCorrection;

    public void MoveToNextRound()
    {
        State = TaskForReviewState.New;
        NextNotification = DateTimeOffset.UtcNow;
    }

    public bool CanMoveToInProgress() => State == TaskForReviewState.New;

    public void MoveToInProgress(TimeSpan notificationInterval)
    {
        State = TaskForReviewState.InProgress;
        SetNextNotificationTime(notificationInterval);
    }
}