namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class TaskForReview
{
    public Guid Id { get; private set; }
    public Guid TeamId { get; private set; }
    public Person Owner { get; private set; } = default!;
    public Person Reviewer { get; private set; } = default!;
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

    public TaskForReview(Guid teamId, Person owner, Person reviewer, long chatId, string description)
        : this()
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

        Id = Guid.NewGuid();
        Created = DateTimeOffset.UtcNow;
        TeamId = teamId;
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Reviewer = reviewer ?? throw new ArgumentNullException(nameof(reviewer));
        ChatId = chatId;
        Description = description;
        State = TaskForReviewState.New;
        NextNotification = DateTimeOffset.UtcNow;
    }

    public TaskForReview Build(Person owner, Person reviewer)
    {
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Reviewer = reviewer ?? throw new ArgumentNullException(nameof(reviewer));

        return this;
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