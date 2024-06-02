using System.Text;
using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMessageBuilder : IReviewMessageBuilder
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ITaskForReviewReader _taskForReviewReader;

    public ReviewMessageBuilder(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        ITaskForReviewReader taskForReviewReader)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
    }

    public async Task<IReadOnlyCollection<NotificationMessage>> Build(
        int messageId,
        TaskForReview taskForReview,
        Person reviewer,
        Person owner,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(reviewer);
        ArgumentNullException.ThrowIfNull(owner);

        var notifications = new List<NotificationMessage>
        {
            await MessageForTeam(taskForReview, reviewer, owner, token),
            await MessageForReviewer(taskForReview, token)
        };

        if (!taskForReview.ReviewerMessageId.HasValue && taskForReview.OriginalReviewerId.HasValue)
            notifications.Add(await HideControlsForReviewer(taskForReview.OriginalReviewerId.Value, messageId, taskForReview, token));
        
        if (taskForReview.State == TaskForReviewState.OnCorrection || messageId == taskForReview.OwnerMessageId)
            notifications.Add(await MessageForOwner(taskForReview, token));
        
        if (taskForReview.State == TaskForReviewState.Accept)
            notifications.Add(await ReviewFinish(taskForReview, token));

        return notifications;
    }

    public async Task<NotificationMessage?> Push(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        if (taskForReview is { State: TaskForReviewState.New or TaskForReviewState.InProgress, ReviewerMessageId: not null })
        {
            var languageId = await _teamAccessor.GetClientLanguage(taskForReview.ReviewerId, token);
            var reviewerNotificationText = await _messageBuilder.Build(Messages.Reviewer_NeedEndReview, languageId);
            return NotificationMessage
                .Create(taskForReview.ReviewerId, reviewerNotificationText)
                .ReplyTo(taskForReview.ReviewerMessageId.Value);
        }
        
        if (taskForReview is { State: TaskForReviewState.OnCorrection, OwnerMessageId: not null })
        {
            var languageId = await _teamAccessor.GetClientLanguage(taskForReview.OwnerId, token);
            var ownerNotificationText = await _messageBuilder.Build(Messages.Reviewer_NeedRevisions, languageId);
            return NotificationMessage
                .Create(taskForReview.OwnerId, ownerNotificationText)
                .ReplyTo(taskForReview.OwnerMessageId.Value);
        }

        return null;
    }
    
    private async Task<NotificationMessage> MessageForTeam(
        TaskForReview taskForReview,
        Person reviewer,
        Person owner,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(reviewer);
        ArgumentNullException.ThrowIfNull(owner);
        
        var hasUsername = !string.IsNullOrWhiteSpace(reviewer.Username);
        var attachedPersonId = hasUsername ? null : (long?)reviewer.Id;
        var text = await _messageBuilder.Build(
            Messages.Reviewer_NewTaskForReview,
            await _teamAccessor.GetClientLanguage(owner.Id, token),
            taskForReview.Description,
            owner.DisplayName,
            hasUsername ? $"@{reviewer.Username}" : reviewer.Name);
        var state = taskForReview.State switch
        {
            TaskForReviewState.New => "⏳",
            TaskForReviewState.InProgress => "🤩",
            TaskForReviewState.OnCorrection => "😱",
            TaskForReviewState.Accept => "👍",
            _ => throw new ArgumentOutOfRangeException($"State {taskForReview.State} out of range for {nameof(TaskForReviewState)}.")
        };
        
        var builder = new StringBuilder();
        builder.AppendLine(text);
        builder.AppendLine(state);
        var message = builder.ToString();
        
        var notification = taskForReview.MessageId.HasValue
            ? NotificationMessage
                .Edit(new ChatMessage(taskForReview.ChatId, taskForReview.MessageId.Value), message)
                .AttachPerson(attachedPersonId)
            : NotificationMessage
                .Create(taskForReview.ChatId, message)
                .AttachPerson(attachedPersonId)
                .AddHandler((c, p) => new AttachMessageCommand(
                    c,
                    taskForReview.Id,
                    int.Parse(p),
                    MessageType.Shared.ToString()));

        return notification;
    }

    private async Task<NotificationMessage> HideControlsForReviewer(
        long reviewerId,
        int messageId,
        TaskForReview taskForReview,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        var languageId = await _teamAccessor.GetClientLanguage(reviewerId, token);
        var message = await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId, taskForReview.Description);
        var notification = NotificationMessage.Edit(new(reviewerId, messageId), message);

        return notification;
    }
    
    private async Task<NotificationMessage> MessageForReviewer(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.ReviewerId, token);
        var message = await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId, taskForReview.Description);
        var notification = taskForReview.ReviewerMessageId.HasValue
            ? NotificationMessage.Edit(new(taskForReview.ReviewerId, taskForReview.ReviewerMessageId.Value), message)
            : NotificationMessage
                .Create(taskForReview.ReviewerId, message)
                .AddHandler((c, p) => new AttachMessageCommand(
                    c,
                    taskForReview.Id,
                    int.Parse(p),
                    MessageType.Reviewer.ToString()));

        if (taskForReview.State == TaskForReviewState.New)
        {
            var inProgressButton = await _messageBuilder.Build(Messages.Reviewer_MoveToInProgress, languageId);
            notification.WithButton(new Button(inProgressButton, $"{CommandList.MoveToInProgress}{taskForReview.Id:N}"));

            var fromDate = DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday);
            var hasReassign = await _taskForReviewReader.HasReassignFromDate(taskForReview.ReviewerId, fromDate, token);
            if (!taskForReview.OriginalReviewerId.HasValue && !hasReassign)
            {
                var reassignReviewButton = await _messageBuilder.Build(Messages.Reviewer_Reassign, languageId);
                notification.WithButton(new Button(reassignReviewButton, $"{CommandList.ReassignReview}{taskForReview.Id:N}"));
            }
        }

        if (taskForReview.State == TaskForReviewState.InProgress)
        {
            var moveToAcceptButton = await _messageBuilder.Build(Messages.Reviewer_MoveToAccept, languageId);
            notification.WithButton(new Button(moveToAcceptButton, $"{CommandList.Accept}{taskForReview.Id:N}"));
        
            var moveToDeclineButton = await _messageBuilder.Build(Messages.Reviewer_MoveToDecline, languageId);
            notification.WithButton(new Button(moveToDeclineButton, $"{CommandList.Decline}{taskForReview.Id:N}"));
        }

        return notification;
    }

    private async Task<NotificationMessage> MessageForOwner(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.OwnerId, token);
        var message = await _messageBuilder.Build(Messages.Reviewer_ReviewDeclined, languageId, taskForReview.Description);
        var notification = taskForReview.OwnerMessageId.HasValue
            ? NotificationMessage.Edit(new(taskForReview.OwnerId, taskForReview.OwnerMessageId.Value), message)
            : NotificationMessage
                .Create(taskForReview.OwnerId, message)
                .AddHandler((c, p) => new AttachMessageCommand(
                    c,
                    taskForReview.Id,
                    int.Parse(p),
                    MessageType.Owner.ToString()));

        if (taskForReview.State == TaskForReviewState.OnCorrection)
        {
            var moveToNextRoundButton = await _messageBuilder.Build(Messages.Reviewer_MoveToNextRound, languageId);
            notification.WithButton(new Button(moveToNextRoundButton,
                $"{CommandList.MoveToNextRound}{taskForReview.Id:N}"));
        }

        return notification;
    }

    private async Task<NotificationMessage> ReviewFinish(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        
        var message = await _messageBuilder.Build(
            Messages.Reviewer_Accepted,
            await _teamAccessor.GetClientLanguage(taskForReview.OwnerId, token),
            taskForReview.Description);
        var notification = NotificationMessage.Create(taskForReview.OwnerId, message);

        return notification;
    }
}