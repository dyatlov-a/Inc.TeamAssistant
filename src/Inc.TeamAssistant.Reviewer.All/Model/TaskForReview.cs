namespace Inc.TeamAssistant.Reviewer.All.Model;

public sealed class TaskForReview
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public PlayerAsOwner Owner { get; private set; } = default!;
    public Guid ReviewerId { get; private set; }
    public PlayerAsReviewer Reviewer { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTimeOffset NextNotification { get; private set; }

    private TaskForReview()
    {
    }

    public TaskForReview(PlayerAsOwner owner, PlayerAsReviewer reviewer, string description)
        : this()
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(description));

        Id = Guid.NewGuid();
        Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        OwnerId = owner.Id;
        Reviewer = reviewer ?? throw new ArgumentNullException(nameof(reviewer));
        ReviewerId = reviewer.Id;
        Description = description;
        IsActive = true;
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

    public void SetNextNotificationTime(TimeSpan notificationInterval)
        => NextNotification = DateTimeOffset.UtcNow.Add(notificationInterval);

    public void MoveToFinish() => IsActive = false;
}