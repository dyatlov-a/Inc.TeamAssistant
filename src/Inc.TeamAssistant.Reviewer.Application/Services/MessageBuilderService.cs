using System.Runtime.CompilerServices;
using System.Text;
using Inc.TeamAssistant.Holidays;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class MessageBuilderService : IMessageBuilderService
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IHolidayService _holidayService;
    private readonly ITaskForReviewReader _taskForReviewReader;

    public MessageBuilderService(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        IHolidayService holidayService,
        ITaskForReviewReader taskForReviewReader)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
    }

    public async Task<NotificationMessage> BuildNewTaskForReview(
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
    
    public async IAsyncEnumerable<NotificationMessage> BuildMoveToInProgress(
        TaskForReview task,
        Person reviewer,
        bool isPush,
        [EnumeratorCancellation]CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);
        
        var languageId = await _teamAccessor.GetClientLanguage(reviewer.Id, token);
        var message = await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId, task.Description);
        var notification = task.ReviewerMessageId.HasValue
            ? NotificationMessage.Edit(new(reviewer.Id, task.ReviewerMessageId.Value), message)
            : NotificationMessage.Create(reviewer.Id, message);

        var inProgressButton = await _messageBuilder.Build(Messages.Reviewer_MoveToInProgress, languageId);
        notification.WithButton(new Button(inProgressButton, $"{CommandList.MoveToInProgress}{task.Id:N}"));

        var fromDate = _holidayService.GetLastDayOfWeek(DayOfWeek.Monday, DateTimeOffset.UtcNow);
        var hasReassign = await _taskForReviewReader.HasReassignFromDate(reviewer.Id, fromDate, token);
        
        if (!task.OriginalReviewerId.HasValue && !hasReassign)
        {
            var reassignReviewButton = await _messageBuilder.Build(Messages.Reviewer_Reassign, languageId);
            notification.WithButton(new Button(reassignReviewButton, $"{CommandList.ReassignReview}{task.Id:N}"));
        }

        if (!task.ReviewerMessageId.HasValue)
            notification.AddHandler((c, p) => new AttachMessageCommand(
                c,
                task.Id,
                int.Parse(p),
                MessageType.Reviewer.ToString()));
        
        yield return notification;
        
        if (isPush && task.ReviewerMessageId.HasValue)
        {
            var reviewerNotificationText = await _messageBuilder.Build(Messages.Reviewer_NeedEndReview, languageId);
            yield return NotificationMessage
                .Create(reviewer.Id, reviewerNotificationText)
                .ReplyTo(task.ReviewerMessageId.Value);
        }
    }

    public async IAsyncEnumerable<NotificationMessage> BuildMoveToReviewActions(
        TaskForReview task,
        Person reviewer,
        bool isPush,
        bool hasActions,
        [EnumeratorCancellation]CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);

        if (!task.ReviewerMessageId.HasValue)
            throw new ApplicationException($"TaskForReview {task.Id} has not value for ReviewerMessageId.");

        var languageId = await _teamAccessor.GetClientLanguage(reviewer.Id, token);
        var message = await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId, task.Description);
        var notification = NotificationMessage.Edit(new(reviewer.Id, task.ReviewerMessageId.Value), message);

        if (hasActions)
        {
            var moveToAcceptButton = await _messageBuilder.Build(Messages.Reviewer_MoveToAccept, languageId);
            notification.WithButton(new Button(moveToAcceptButton, $"{CommandList.Accept}{task.Id:N}"));
        
            var moveToDeclineButton = await _messageBuilder.Build(Messages.Reviewer_MoveToDecline, languageId);
            notification.WithButton(new Button(moveToDeclineButton, $"{CommandList.Decline}{task.Id:N}"));
        }

        yield return notification;
        
        if (isPush)
        {
            var reviewerNotificationText = await _messageBuilder.Build(Messages.Reviewer_NeedEndReview, languageId);
            yield return NotificationMessage
                .Create(reviewer.Id, reviewerNotificationText)
                .ReplyTo(task.ReviewerMessageId.Value);
        }
    }

    public async IAsyncEnumerable<NotificationMessage> BuildMoveToNextRound(
        TaskForReview task,
        Person owner,
        bool isPush,
        bool hasActions,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(owner);
        
        var languageId = await _teamAccessor.GetClientLanguage(owner.Id, token);
        var message = await _messageBuilder.Build(Messages.Reviewer_ReviewDeclined, languageId, task.Description);
        var notification = task.OwnerMessageId.HasValue
            ? NotificationMessage.Edit(new(owner.Id, task.OwnerMessageId.Value), message)
            : NotificationMessage.Create(owner.Id, message);

        if (hasActions)
        {
            var moveToNextRoundButton = await _messageBuilder.Build(Messages.Reviewer_MoveToNextRound, languageId);
            notification.WithButton(new Button(moveToNextRoundButton, $"{CommandList.MoveToNextRound}{task.Id:N}"));
        }

        if (!task.OwnerMessageId.HasValue)
            notification.AddHandler((c, p) => new AttachMessageCommand(
                c,
                task.Id,
                int.Parse(p),
                MessageType.Owner.ToString()));
        
        yield return notification;
        
        if (isPush && task.OwnerMessageId.HasValue)
        {
            var ownerNotificationText = await _messageBuilder.Build(Messages.Reviewer_NeedRevisions, languageId);
            yield return NotificationMessage
                .Create(owner.Id, ownerNotificationText)
                .ReplyTo(task.OwnerMessageId.Value);
        }
    }

    public async Task<NotificationMessage> BuildReviewAccepted(
        TaskForReview task,
        Person owner,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(owner);
        
        var message = await _messageBuilder.Build(
            Messages.Reviewer_Accepted,
            await _teamAccessor.GetClientLanguage(owner.Id, token),
            task.Description);
        var notification = NotificationMessage.Create(owner.Id, message);

        return notification;
    }
}