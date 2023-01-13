namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class TaskForReview
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public PlayerAsOwner Owner { get; private set; } = default!;
    public Guid ReviewerId { get; private set; }
    public PlayerAsReviewer Reviewer { get; private set; } = default!;
    public long ChatId { get; private set; }
    public string Description { get; private set; } = default!;
    public TaskForReviewState State { get; private set; }
    public DateTimeOffset NextNotification { get; private set; }
    public DateTimeOffset? AcceptDate { get; private set; }
    public int? MessageId { get; private set; }

    private TaskForReview()
    {
    }

    public TaskForReview(PlayerAsOwner owner, PlayerAsReviewer reviewer, long chatId, string description)
        : this()
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

        Id = Guid.NewGuid();
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        OwnerId = owner.Id;
        Reviewer = reviewer ?? throw new ArgumentNullException(nameof(reviewer));
        ChatId = chatId;
        ReviewerId = reviewer.Id;
        Description = description;
        State = TaskForReviewState.New;
        NextNotification = DateTimeOffset.UtcNow;
    }

    public TaskForReview Build(PlayerAsOwner owner, PlayerAsReviewer reviewer)
    {
        if (owner is null)
            throw new ArgumentNullException(nameof(owner));
        if (reviewer is null)
            throw new ArgumentNullException(nameof(reviewer));
        if (OwnerId != owner.Id)
            throw new ApplicationException("Map Owner from other task.");
        if (ReviewerId != reviewer.Id)
            throw new ApplicationException("Map Reviewer from other task.");

        Owner = owner;
        Reviewer = reviewer;

        return this;
    }

    public void AttachMessage(int messageId) => MessageId = messageId;

    public void SetNextNotificationTime(TimeSpan notificationInterval)
    {
        var interval = State == TaskForReviewState.InProgress ? notificationInterval * 2 : notificationInterval;
        NextNotification = DateTimeOffset.UtcNow.Add(interval);
    }

    public void Accept()
    {
        State = TaskForReviewState.IsArchived;
        AcceptDate = DateTimeOffset.UtcNow;
    }

    public void Decline()
    {
        State = TaskForReviewState.OnCorrection;
        NextNotification = DateTimeOffset.UtcNow;
    }

    public void MoveToNextRound()
    {
        State = TaskForReviewState.New;
        NextNotification = DateTimeOffset.UtcNow;
    }

    public void MoveToInProgress(TimeSpan notificationInterval)
    {
        State = TaskForReviewState.InProgress;
        SetNextNotificationTime(notificationInterval);
    }
}