using System.Text;
using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachPreview;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReviewMessageBuilder : IReviewMessageBuilder
{
    private readonly IMessageBuilder _messageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly ITaskForReviewReader _taskForReviewReader;
    private readonly IReviewMetricsProvider _metricsProvider;
    private readonly ReviewTeamMetricsFactory _metricsFactory;
    private readonly DraftTaskForReviewService _service;
    private readonly ReviewStatsBuilder _reviewStatsBuilder;

    public ReviewMessageBuilder(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        ITaskForReviewReader taskForReviewReader,
        IReviewMetricsProvider metricsProvider,
        ReviewTeamMetricsFactory metricsFactory,
        DraftTaskForReviewService service,
        ReviewStatsBuilder reviewStatsBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
        _metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _reviewStatsBuilder = reviewStatsBuilder ?? throw new ArgumentNullException(nameof(reviewStatsBuilder));
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

        var metricsByTeam = _metricsProvider.Get(taskForReview.TeamId);
        var metricsByTask = await _metricsFactory.Create(taskForReview, token);

        var notifications = new List<NotificationMessage>
        {
            await MessageForTeam(taskForReview, reviewer, owner, metricsByTeam, metricsByTask, token),
            await MessageForReviewer(taskForReview, metricsByTeam, metricsByTask, token)
        };

        if (!taskForReview.ReviewerMessageId.HasValue && taskForReview.OriginalReviewerId.HasValue)
            notifications.Add(await HideControlsForReviewer(
                taskForReview.OriginalReviewerId.Value,
                messageId,
                taskForReview,
                token));
        
        if (taskForReview.State == TaskForReviewState.OnCorrection || messageId == taskForReview.OwnerMessageId)
            notifications.Add(await MessageForOwner(taskForReview, metricsByTeam, metricsByTask, token));
        
        if (taskForReview.State == TaskForReviewState.Accept)
            notifications.Add(await ReviewFinish(taskForReview, token));

        return notifications;
    }

    public async Task<NotificationMessage?> Push(TaskForReview taskForReview, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);

        var totalTime = taskForReview.GetTotalTime(DateTimeOffset.UtcNow);

        return taskForReview switch
        {
            { State: TaskForReviewState.New or TaskForReviewState.InProgress, ReviewerMessageId: not null } =>
                await CreatePushMessage(
                    taskForReview.BotId,
                    taskForReview.ReviewerId,
                    taskForReview.ReviewerMessageId.Value,
                    Messages.Reviewer_NeedEndReview,
                    totalTime,
                    token),
            { State: TaskForReviewState.OnCorrection, OwnerMessageId: not null } =>
                await CreatePushMessage(
                    taskForReview.BotId,
                    taskForReview.OwnerId,
                    taskForReview.OwnerMessageId.Value,
                    Messages.Reviewer_NeedCorrection,
                    totalTime,
                    token),
            _ => null
        };
    }

    public async Task<NotificationMessage> Build(
        DraftTaskForReview draft,
        LanguageId languageId,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var teamContext = await _teamAccessor.GetTeamContext(draft.TeamId, token);
        var reviewTargetMessageTemplate = await _messageBuilder.Build(
            Messages.Reviewer_PreviewReviewerTemplate,
            languageId);
        var messageBuilder = new StringBuilder();
        
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_PreviewTitle, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(draft.Description);
        messageBuilder.AppendLine();

        if (draft.TargetPersonId.HasValue)
        {
            var reviewTarget = await _teamAccessor.GetPerson(draft.TargetPersonId.Value, token);
            messageBuilder.AppendLine(string.Format(reviewTargetMessageTemplate, reviewTarget.DisplayName));
            
            if (!await _service.HasTeammate(draft.TeamId, draft.TargetPersonId.Value, token))
            {
                messageBuilder.AppendLine();
                messageBuilder.Append('❗');
                messageBuilder.Append(await _messageBuilder.Build(
                    Messages.Reviewer_PreviewCheckTeammate,
                    languageId,
                    reviewTarget.DisplayName,
                    teamContext.TeamName));
                messageBuilder.AppendLine();
            }
        }
        else
            messageBuilder.AppendLine(string.Format(reviewTargetMessageTemplate, teamContext.TeamName));
        
        if (!_service.HasDescriptionAndLinks(draft.Description))
        {
            messageBuilder.AppendLine();
            messageBuilder.Append('❗');
            messageBuilder.Append(await _messageBuilder.Build(Messages.Reviewer_PreviewCheckDescription, languageId));
            messageBuilder.AppendLine();
        }
        
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_PreviewEditHelp, languageId));
        var message = messageBuilder.ToString();

        var notificationMessage = draft.PreviewMessageId.HasValue
            ? NotificationMessage.Edit(new ChatMessage(draft.ChatId, draft.PreviewMessageId.Value), message)
            : NotificationMessage.Create(draft.ChatId, message)
                .ReplyTo(draft.MessageId)
                .AddHandler((c, p) => new AttachPreviewCommand(c, draft.Id, int.Parse(p)));
        
        return notificationMessage
            .WithButton(new Button(
                await _messageBuilder.Build(Messages.Reviewer_PreviewMoveToReview, languageId),
                $"{CommandList.MoveToReview}{draft.Id:N}"))
            .WithButton(new Button(
                await _messageBuilder.Build(Messages.Reviewer_PreviewRemoveDraft, languageId),
                $"{CommandList.RemoveDraft}{draft.Id:N}"));
    }

    private async Task<NotificationMessage?> CreatePushMessage(
        Guid botId,
        long personId,
        int messageId,
        MessageId messageTextId,
        TimeSpan workTimeTotal,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(messageTextId);
        
        var languageId = await _teamAccessor.GetClientLanguage(botId, personId, token);
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(messageTextId, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_TotalTime,
            languageId,
            workTimeTotal.ToString(GlobalSettings.TimeFormat)));
        
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
        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.BotId, owner.Id, token);
        var state = taskForReview.State switch
        {
            TaskForReviewState.New => "⏳",
            TaskForReviewState.InProgress => "🤩",
            TaskForReviewState.OnCorrection => "😱",
            TaskForReviewState.Accept => "🤝",
            _ => throw new ArgumentOutOfRangeException(
                nameof(TaskForReviewState),
                taskForReview.State,
                "State out of range.")
        };
        var reviewerTargetMessageKey = taskForReview.HasConcreteReviewer
            ? Messages.Reviewer_TargetManually
            : Messages.Reviewer_TargetAutomatically;
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NewTaskForReview, languageId));
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_Owner, languageId, owner.DisplayName));
        
        messageBuilder.Append(await _messageBuilder.Build(reviewerTargetMessageKey, languageId));
        reviewer.Append(messageBuilder, (p, o) => attachPersons += n => n.AttachPerson(p, o));
        messageBuilder.AppendLine();
        
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        messageBuilder.AppendLine(state);
        
        var stats = await _reviewStatsBuilder.Build(
            taskForReview,
            metricsByTeam,
            metricsByTask,
            languageId,
            hasReviewMetrics: true,
            hasCorrectionMetrics: true);
        messageBuilder.Append(stats);
        
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

        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.BotId, reviewerId, token);
        
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
        
        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.BotId, taskForReview.ReviewerId, token);
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        
        var stats = await _reviewStatsBuilder.Build(
            taskForReview,
            metricsByTeam,
            metricsByTask,
            languageId,
            hasReviewMetrics: true,
            hasCorrectionMetrics: false);
        messageBuilder.Append(stats);
        
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
            notification.WithButton(new Button(
                inProgressButton,
                $"{CommandList.MoveToInProgress}{taskForReview.Id:N}"));

            var fromDate = DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday);
            var hasReassign = await _taskForReviewReader.HasReassignFromDate(taskForReview.ReviewerId, fromDate, token);
            if (!taskForReview.OriginalReviewerId.HasValue && !hasReassign)
            {
                var reassignReviewButton = await _messageBuilder.Build(Messages.Reviewer_Reassign, languageId);
                notification.WithButton(new Button(
                    reassignReviewButton,
                    $"{CommandList.ReassignReview}{taskForReview.Id:N}"));
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

        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.BotId, taskForReview.OwnerId, token);
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_ReviewDeclined, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        
        var stats = await _reviewStatsBuilder.Build(
            taskForReview,
            metricsByTeam,
            metricsByTask,
            languageId,
            hasReviewMetrics: false,
            hasCorrectionMetrics: true);
        messageBuilder.Append(stats);
        
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

        var languageId = await _teamAccessor.GetClientLanguage(taskForReview.BotId, taskForReview.OwnerId, token);
        var totalTime = taskForReview.GetTotalTime(DateTimeOffset.UtcNow);
        
        var messageBuilder = new StringBuilder();
        messageBuilder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_Accepted, languageId));
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(taskForReview.Description);
        messageBuilder.AppendLine();
        messageBuilder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_TotalTime,
            languageId,
            totalTime.ToString(GlobalSettings.TimeFormat)));

        return NotificationMessage.Create(taskForReview.OwnerId, messageBuilder.ToString());
    }
}