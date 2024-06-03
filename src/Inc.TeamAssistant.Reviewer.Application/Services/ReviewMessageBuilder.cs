using System.Text;
using Inc.TeamAssistant.Holidays;
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
    private const string TimeFormat = @"d\.hh\:mm\:ss";
    
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly IHolidayService _holidayService;
    private readonly IReviewMetricsProvider _reviewMetricsProvider;

    public ReviewMessageBuilder(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        ITaskForReviewReader taskForReviewReader,
        IHolidayService holidayService,
        IReviewMetricsProvider reviewMetricsProvider)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _reviewMetricsProvider = reviewMetricsProvider ?? throw new ArgumentNullException(nameof(reviewMetricsProvider));
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

        var metricsByTeam = _reviewMetricsProvider.Get(taskForReview.TeamId);
        var metricsByTask = await _reviewMetricsProvider.Create(taskForReview, token);

        var notifications = new List<NotificationMessage>
        {
            await MessageForTeam(taskForReview, reviewer, owner, metricsByTeam, metricsByTask, token),
            await MessageForReviewer(taskForReview, metricsByTeam, metricsByTask, token)
        };

        if (!taskForReview.ReviewerMessageId.HasValue && taskForReview.OriginalReviewerId.HasValue)
            notifications.Add(await HideControlsForReviewer(taskForReview.OriginalReviewerId.Value, messageId, taskForReview, token));
        
        if (taskForReview.State == TaskForReviewState.OnCorrection || messageId == taskForReview.OwnerMessageId)
            notifications.Add(await MessageForOwner(taskForReview, metricsByTeam, metricsByTask, token));
        
        if (taskForReview.State == TaskForReviewState.Accept)
            notifications.Add(await ReviewFinish(taskForReview, token));

        return notifications;
    }

    public async Task<NotificationMessage?> Push(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var workTimeTotal = await _holidayService.CalculateWorkTime(
            taskForReview.Created,
            DateTimeOffset.UtcNow,
            token);
        
        if (taskForReview is { State: TaskForReviewState.New or TaskForReviewState.InProgress, ReviewerMessageId: not null })
        {
            var languageId = await _teamAccessor.GetClientLanguage(taskForReview.ReviewerId, token);
            var reviewerNotificationText = await _messageBuilder.Build(
                Messages.Reviewer_NeedEndReview,
                languageId,
                workTimeTotal.ToString(TimeFormat));
            return NotificationMessage
                .Create(taskForReview.ReviewerId, reviewerNotificationText)
                .ReplyTo(taskForReview.ReviewerMessageId.Value);
        }
        
        if (taskForReview is { State: TaskForReviewState.OnCorrection, OwnerMessageId: not null })
        {
            var languageId = await _teamAccessor.GetClientLanguage(taskForReview.OwnerId, token);
            var ownerNotificationText = await _messageBuilder.Build(
                Messages.Reviewer_NeedRevisions,
                languageId,
                workTimeTotal.ToString(TimeFormat));
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
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(reviewer);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);

        var languageId = await _teamAccessor.GetClientLanguage(owner.Id, token);
        var hasUsername = !string.IsNullOrWhiteSpace(reviewer.Username);
        var attachedPersonId = hasUsername ? null : (long?)reviewer.Id;
        var text = await _messageBuilder.Build(
            Messages.Reviewer_NewTaskForReview,
            languageId,
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
        await AddStats(
            builder,
            taskForReview,
            metricsByTeam,
            metricsByTask,
            languageId,
            hasReviewMetrics: true,
            hasCorrectionMetrics: true);
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

    private async Task<NotificationMessage> MessageForReviewer(
        TaskForReview taskForReview,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);
        
        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.ReviewerId, token);
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_NeedReview,
            languageId,
            taskForReview.Description));
        await AddStats(
            messageBuilder,
            taskForReview,
            metricsByTeam,
            metricsByTask,
            languageId,
            hasReviewMetrics: true,
            hasCorrectionMetrics: false);
        var message = messageBuilder.ToString();
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

    private async Task<NotificationMessage> MessageForOwner(
        TaskForReview taskForReview,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);

        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.OwnerId, token);
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_ReviewDeclined,
            languageId,
            taskForReview.Description));
        await AddStats(
            messageBuilder,
            taskForReview,
            metricsByTeam,
            metricsByTask,
            languageId,
            hasReviewMetrics: false,
            hasCorrectionMetrics: true);
        var message = messageBuilder.ToString();
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
        
        var workTimeTotal = await _holidayService.CalculateWorkTime(
            taskForReview.Created,
            DateTimeOffset.UtcNow,
            token);
        
        var message = await _messageBuilder.Build(
            Messages.Reviewer_Accepted,
            await _teamAccessor.GetClientLanguage(taskForReview.OwnerId, token),
            taskForReview.Description,
            workTimeTotal.ToString(TimeFormat));
        var notification = NotificationMessage.Create(taskForReview.OwnerId, message);

        return notification;
    }
    
    private async Task AddStats(
        StringBuilder builder,
        TaskForReview taskForReview,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        LanguageId languageId,
        bool hasReviewMetrics,
        bool hasCorrectionMetrics)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);
        ArgumentNullException.ThrowIfNull(languageId);

        const string firstTouchIcon = "🕛 ";
        const string reviewTouchIcon = "🔍 ";
        const string correctionTouchIcon = "💻 ";
        
        var attempts = taskForReview.GetAttempts();
        if (attempts.HasValue)
            builder.AppendLine(await _messageBuilder.Build(
                Messages.Reviewer_StatsAttempts,
                languageId,
                attempts.Value));
        
        if (taskForReview.State == TaskForReviewState.Accept)
        {
            if (hasReviewMetrics)
            {
                builder.AppendLine(firstTouchIcon + await _messageBuilder.Build(
                    Messages.Reviewer_StatsFirstTouch,
                    languageId,
                    metricsByTask.FirstTouch.ToString(TimeFormat),
                    metricsByTeam.FirstTouch.ToString(TimeFormat)));
                builder.AppendLine(reviewTouchIcon + await _messageBuilder.Build(
                    Messages.Reviewer_StatsReview,
                    languageId,
                    metricsByTask.Review.ToString(TimeFormat),
                    metricsByTeam.Review.ToString(TimeFormat)));
            }

            if (hasCorrectionMetrics && attempts.HasValue)
                builder.AppendLine(correctionTouchIcon + await _messageBuilder.Build(
                    Messages.Reviewer_StatsCorrection,
                    languageId,
                    metricsByTask.Correction.ToString(TimeFormat),
                    metricsByTeam.Correction.ToString(TimeFormat)));
        }
        else
        {
            if (hasReviewMetrics)
            {
                builder.AppendLine(firstTouchIcon + await _messageBuilder.Build(
                    Messages.Reviewer_StatsFirstTouchAverage,
                    languageId,
                    metricsByTeam.FirstTouch.ToString(TimeFormat)));
                builder.AppendLine(reviewTouchIcon + await _messageBuilder.Build(
                    Messages.Reviewer_StatsReviewAverage,
                    languageId,
                    metricsByTeam.Review.ToString(TimeFormat)));
            }
            
            if (hasCorrectionMetrics && attempts.HasValue)
                builder.AppendLine(correctionTouchIcon + await _messageBuilder.Build(
                    Messages.Reviewer_StatsCorrectionAverage,
                    languageId,
                    metricsByTeam.Correction.ToString(TimeFormat)));
        }
    }
}