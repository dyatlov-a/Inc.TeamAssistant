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
    private readonly ReviewTeamMetricsFactory _metricsFactory;

    public ReviewMessageBuilder(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        ITaskForReviewReader taskForReviewReader,
        IHolidayService holidayService,
        IReviewMetricsProvider reviewMetricsProvider,
        ReviewTeamMetricsFactory metricsFactory)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _holidayService = holidayService ?? throw new ArgumentNullException(nameof(holidayService));
        _reviewMetricsProvider = reviewMetricsProvider ?? throw new ArgumentNullException(nameof(reviewMetricsProvider));
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
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
        var metricsByTask = await _metricsFactory.Create(taskForReview, token);

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

        return taskForReview switch
        {
            { State: TaskForReviewState.New or TaskForReviewState.InProgress, ReviewerMessageId: not null } =>
                await CreatePushMessage(
                    taskForReview.ReviewerId,
                    taskForReview.ReviewerMessageId.Value,
                    workTimeTotal,
                    token),
            { State: TaskForReviewState.OnCorrection, OwnerMessageId: not null } =>
                await CreatePushMessage(
                    taskForReview.OwnerId,
                    taskForReview.OwnerMessageId.Value,
                    workTimeTotal,
                    token),
            _ => null
        };
    }

    private async Task<NotificationMessage?> CreatePushMessage(
        long personId,
        int messageId,
        TimeSpan workTimeTotal,
        CancellationToken token)
    {
        var languageId = await _teamAccessor.GetClientLanguage(personId, token);
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NeedEndReview, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_TotalTime,
            languageId,
            workTimeTotal.ToString(TimeFormat)));
        
        return NotificationMessage
            .Create(personId, messageBuilder.ToString())
            .ReplyTo(messageId);
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

        Func<NotificationMessage, NotificationMessage> attachPersons = n => n;
        var languageId = await _teamAccessor.GetClientLanguage(owner.Id, token);
        var state = taskForReview.State switch
        {
            TaskForReviewState.New => "⏳",
            TaskForReviewState.InProgress => "🤩",
            TaskForReviewState.OnCorrection => "😱",
            TaskForReviewState.Accept => "🤝",
            _ => throw new ArgumentOutOfRangeException($"State {taskForReview.State} out of range for {nameof(TaskForReviewState)}.")
        };
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NewTaskForReview, languageId));
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_Owner, languageId, owner.DisplayName));
        
        messageBuilder.Append(await _messageBuilder.Build(Messages.Reviewer_Target, languageId));
        reviewer.Append(messageBuilder, (p, o) => attachPersons += n => n.AttachPerson(p, o));
        messageBuilder.AppendLine();
        
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        messageBuilder.AppendLine(state);
        
        await AddStats(
            messageBuilder,
            taskForReview,
            metricsByTeam,
            metricsByTask,
            languageId,
            hasReviewMetrics: true,
            hasCorrectionMetrics: true);
        var message = messageBuilder.ToString();
        
        var notification = taskForReview.MessageId.HasValue
            ? NotificationMessage.Edit(new ChatMessage(taskForReview.ChatId, taskForReview.MessageId.Value), message)
            : NotificationMessage
                .Create(taskForReview.ChatId, message)
                .AddHandler((c, p) => new AttachMessageCommand(
                    c,
                    taskForReview.Id,
                    int.Parse(p),
                    MessageType.Shared.ToString()));

        return attachPersons(notification);
    }

    private async Task<NotificationMessage> HideControlsForReviewer(
        long reviewerId,
        int messageId,
        TaskForReview taskForReview,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var languageId = await _teamAccessor.GetClientLanguage(reviewerId, token);
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        
        return NotificationMessage.Edit(new(reviewerId, messageId), messageBuilder.ToString());
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
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        
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
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_ReviewDeclined, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        
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

        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.OwnerId, token);
        var workTimeTotal = await _holidayService.CalculateWorkTime(
            taskForReview.Created,
            DateTimeOffset.UtcNow,
            token);
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_Accepted, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_TotalTime,
            languageId,
            workTimeTotal.ToString(TimeFormat)));

        return NotificationMessage.Create(taskForReview.OwnerId, messageBuilder.ToString());
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

        builder.AppendLine();
        var attempts = taskForReview.GetAttempts();
        if (attempts.HasValue)
            builder.AppendLine(await _messageBuilder.Build(
                Messages.Reviewer_StatsAttempts,
                languageId,
                attempts.Value));
        
        if (taskForReview.State == TaskForReviewState.Accept)
        {
            const string trendUp = "👍";
            const string trendDown = "👎";
            
            if (hasReviewMetrics)
            {
                var firstTouchTrend = metricsByTask.FirstTouch <= metricsByTeam.FirstTouch ? trendUp : trendDown;
                var firstTouchMessage = await _messageBuilder.Build(
                    Messages.Reviewer_StatsFirstTouch,
                    languageId,
                    metricsByTask.FirstTouch.ToString(TimeFormat),
                    metricsByTeam.FirstTouch.ToString(TimeFormat));
                builder.AppendLine($"{firstTouchMessage} {firstTouchTrend}");
                
                var reviewTrend = metricsByTask.Review <= metricsByTeam.Review ? trendUp : trendDown;
                var reviewMessage = await _messageBuilder.Build(
                    Messages.Reviewer_StatsReview,
                    languageId,
                    metricsByTask.Review.ToString(TimeFormat),
                    metricsByTeam.Review.ToString(TimeFormat));
                builder.AppendLine($"{reviewMessage} {reviewTrend}");
            }

            if (hasCorrectionMetrics && attempts.HasValue)
            {
                var correctionTrend = metricsByTask.Correction <= metricsByTeam.Correction ? trendUp : trendDown;
                var correctionMessage = await _messageBuilder.Build(
                    Messages.Reviewer_StatsCorrection,
                    languageId,
                    metricsByTask.Correction.ToString(TimeFormat),
                    metricsByTeam.Correction.ToString(TimeFormat));
                builder.AppendLine($"{correctionMessage} {correctionTrend}");
            }
        }
        else
        {
            if (hasReviewMetrics)
            {
                builder.AppendLine(await _messageBuilder.Build(
                    Messages.Reviewer_StatsFirstTouchAverage,
                    languageId,
                    metricsByTeam.FirstTouch.ToString(TimeFormat)));
                builder.AppendLine(await _messageBuilder.Build(
                    Messages.Reviewer_StatsReviewAverage,
                    languageId,
                    metricsByTeam.Review.ToString(TimeFormat)));
            }
            
            if (hasCorrectionMetrics && attempts.HasValue)
                builder.AppendLine(await _messageBuilder.Build(
                    Messages.Reviewer_StatsCorrectionAverage,
                    languageId,
                    metricsByTeam.Correction.ToString(TimeFormat)));
        }
    }
}