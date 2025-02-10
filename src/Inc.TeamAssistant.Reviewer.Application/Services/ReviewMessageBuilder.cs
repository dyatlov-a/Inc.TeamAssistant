using System.Text;
using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
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
    private readonly ITaskForReviewReader _taskReader;
    private readonly IReviewMetricsProvider _metricsProvider;
    private readonly ReviewTeamMetricsFactory _metricsFactory;
    private readonly DraftTaskForReviewService _draftService;

    public ReviewMessageBuilder(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        ITaskForReviewReader taskReader,
        IReviewMetricsProvider metricsProvider,
        ReviewTeamMetricsFactory metricsFactory,
        DraftTaskForReviewService draftService)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _taskReader = taskReader ?? throw new ArgumentNullException(nameof(taskReader));
        _metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
        _draftService = draftService ?? throw new ArgumentNullException(nameof(draftService));
    }

    public async Task<IReadOnlyCollection<NotificationMessage>> Build(
        int messageId,
        TaskForReview task,
        Person reviewer,
        Person owner,
        BotContext botContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(botContext);

        var metricsByTeam = _metricsProvider.Get(task.TeamId);
        var metricsByTask = await _metricsFactory.Create(task, token);
        var hasOwnerAction = messageId == task.OwnerMessageId;

        var notifications = new List<NotificationMessage>
        {
            await MessageForTeam(task, reviewer, owner, metricsByTeam, metricsByTask, token),
            await MessageForReviewer(task, botContext, metricsByTeam, metricsByTask, token)
        };

        if (!task.ReviewerMessageId.HasValue && task.OriginalReviewerId.HasValue)
            notifications.Add(await HideControlsForOriginalReviewer(
                task.OriginalReviewerId.Value,
                messageId,
                task,
                token));
        
        if (task.State == TaskForReviewState.OnCorrection || hasOwnerAction)
            notifications.Add(await MessageForOwner(task, metricsByTeam, metricsByTask, token));
        
        if (task.State == TaskForReviewState.Accept)
            notifications.Add(await ReviewFinish(task, token));

        return notifications;
    }

    public async Task<NotificationMessage?> Push(TaskForReview task, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);

        var totalTime = task.GetTotalTime(DateTimeOffset.UtcNow);

        return task switch
        {
            { State: TaskForReviewState.New or TaskForReviewState.InProgress, ReviewerMessageId: not null } =>
                await CreatePushMessage(
                    task.BotId,
                    task.ReviewerId,
                    task.ReviewerMessageId.Value,
                    Messages.Reviewer_NeedEndReview,
                    totalTime,
                    token),
            { State: TaskForReviewState.OnCorrection, OwnerMessageId: not null } =>
                await CreatePushMessage(
                    task.BotId,
                    task.OwnerId,
                    task.OwnerMessageId.Value,
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
        var builder = new StringBuilder();
        
        builder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_PreviewTitle, languageId));
        builder.AppendLine();
        builder.AppendLine(draft.Description);
        builder.AppendLine();

        if (draft.TargetPersonId.HasValue)
        {
            var reviewTarget = await _teamAccessor.GetPerson(draft.TargetPersonId.Value, token);
            builder.AppendLine(string.Format(reviewTargetMessageTemplate, reviewTarget.DisplayName));
            
            if (!await _draftService.HasTeammate(draft.TeamId, draft.TargetPersonId.Value, token))
            {
                builder.AppendLine();
                builder.Append(Icons.Alert);
                builder.Append(await _messageBuilder.Build(
                    Messages.Reviewer_PreviewCheckTeammate,
                    languageId,
                    reviewTarget.DisplayName,
                    teamContext.Name));
                builder.AppendLine();
            }
        }
        else
            builder.AppendLine(string.Format(reviewTargetMessageTemplate, teamContext.Name));
        
        if (!_draftService.HasDescriptionAndLinks(draft.Description))
        {
            builder.AppendLine();
            builder.Append(Icons.Alert);
            builder.Append(await _messageBuilder.Build(Messages.Reviewer_PreviewCheckDescription, languageId));
            builder.AppendLine();
        }
        
        builder.AppendLine();
        builder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_PreviewEditHelp, languageId));
        var message = builder.ToString();

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
        
        var builder = new StringBuilder();
        builder.AppendLine(await _messageBuilder.Build(messageTextId, languageId));
        builder.AppendLine();
        builder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_TotalTime,
            languageId,
            workTimeTotal.ToString(GlobalSettings.TimeFormat)));
        
        return NotificationMessage
            .Create(personId, builder.ToString())
            .ReplyTo(messageId);
    }
    
    private async Task<NotificationMessage> MessageForTeam(
        TaskForReview task,
        Person reviewer,
        Person owner,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);

        Func<NotificationMessage, NotificationMessage> attachPersons = n => n;
        var ownerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, owner.Id, token);
        var reviewerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, reviewer.Id, token);
        var reviewerTargetMessageKey = (task.OriginalReviewerId.HasValue, task.Strategy) switch
        {
            (true, _) => Messages.Reviewer_TargetReassigned,
            (_, NextReviewerType.Target) => Messages.Reviewer_TargetManually,
            (_, _) => Messages.Reviewer_TargetAutomatically
        };
        
        var builder = new StringBuilder();
        builder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NewTaskForReview, ownerLanguageId));
        builder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_Owner, ownerLanguageId, owner.DisplayName));
        
        builder.Append(await _messageBuilder.Build(reviewerTargetMessageKey, ownerLanguageId));
        reviewer.Append(builder, (p, o) => attachPersons += n => n.AttachPerson(p, reviewerLanguageId, o));
        builder.AppendLine();
        
        builder.AppendLine();
        builder.AppendLine(task.Description);
        builder.AppendLine(StateAsIcon(task));

        var stats = await ReviewStatsBuilder
            .Create(_messageBuilder)
            .WithReviewMetrics()
            .WithCorrectionMetrics()
            .Build(task, metricsByTeam, metricsByTask, ownerLanguageId);
        builder.Append(stats);
        
        var message = builder.ToString();
        var notification = task.MessageId.HasValue
            ? NotificationMessage.Edit(new ChatMessage(task.ChatId, task.MessageId.Value), message)
            : NotificationMessage
                .Create(task.ChatId, message)
                .AddHandler((c, p) => new AttachMessageCommand(
                    c,
                    task.Id,
                    int.Parse(p),
                    MessageType.Shared.ToString()));

        return attachPersons(notification);
    }

    private async Task<NotificationMessage> HideControlsForOriginalReviewer(
        long reviewerId,
        int messageId,
        TaskForReview task,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);

        var languageId = await _teamAccessor.GetClientLanguage(task.BotId, reviewerId, token);
        
        var builder = new StringBuilder();
        builder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId));
        builder.AppendLine();
        builder.AppendLine(task.Description);
        
        return NotificationMessage.Edit(new(reviewerId, messageId), builder.ToString());
    }

    private async Task<NotificationMessage> MessageForReviewer(
        TaskForReview task,
        BotContext botContext,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(botContext);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);
        
        var languageId = await _teamAccessor.GetClientLanguage(task.BotId, task.ReviewerId, token);
        var builder = new StringBuilder();
        builder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_NeedReview, languageId));
        builder.AppendLine();
        builder.AppendLine(task.Description);
        
        var stats = await ReviewStatsBuilder
            .Create(_messageBuilder)
            .WithReviewMetrics()
            .Build(task, metricsByTeam, metricsByTask, languageId);
        builder.Append(stats);
        
        var message = builder.ToString();
        var notification = task.ReviewerMessageId.HasValue
            ? NotificationMessage.Edit(new(task.ReviewerId, task.ReviewerMessageId.Value), message)
            : NotificationMessage
                .Create(task.ReviewerId, message)
                .AddHandler((c, p) => new AttachMessageCommand(
                    c,
                    task.Id,
                    int.Parse(p),
                    MessageType.Reviewer.ToString()));

        if (task.State == TaskForReviewState.New)
        {
            var inProgressButton = await _messageBuilder.Build(Messages.Reviewer_MoveToInProgress, languageId);
            notification.WithButton(new Button(
                inProgressButton,
                $"{CommandList.MoveToInProgress}{task.Id:N}"));

            var fromDate = DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday);
            var hasReassign = await _taskReader.HasReassignFromDate(task.ReviewerId, fromDate, token);
            if (!task.OriginalReviewerId.HasValue && !hasReassign)
            {
                var reassignReviewButton = await _messageBuilder.Build(Messages.Reviewer_Reassign, languageId);
                notification.WithButton(new Button(
                    reassignReviewButton,
                    $"{CommandList.ReassignReview}{task.Id:N}"));
            }
        }

        if (task.State == TaskForReviewState.InProgress)
        {
            var moveToAcceptButton = await _messageBuilder.Build(Messages.Reviewer_MoveToAccept, languageId);
            notification.WithButton(new Button(moveToAcceptButton, $"{CommandList.Accept}{task.Id:N}"));

            if (botContext.CanAcceptWithComments())
            {
                var moveToAcceptWithCommentsButton = await _messageBuilder.Build(Messages.Reviewer_MoveToAcceptWithComments, languageId);
                notification.WithButton(new Button(moveToAcceptWithCommentsButton, $"{CommandList.AcceptWithComments}{task.Id:N}"));
            }
            
            var moveToDeclineButton = await _messageBuilder.Build(Messages.Reviewer_MoveToDecline, languageId);
            notification.WithButton(new Button(moveToDeclineButton, $"{CommandList.Decline}{task.Id:N}"));
        }

        return notification;
    }

    private async Task<NotificationMessage> MessageForOwner(
        TaskForReview task,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);

        var languageId = await _teamAccessor.GetClientLanguage(task.BotId, task.OwnerId, token);
        var builder = new StringBuilder();
        builder.AppendLine(await _messageBuilder.Build(Messages.Reviewer_ReviewDeclined, languageId));
        builder.AppendLine();
        builder.AppendLine(task.Description);
        
        var stats = await ReviewStatsBuilder
            .Create(_messageBuilder)
            .WithCorrectionMetrics()
            .Build(task, metricsByTeam, metricsByTask, languageId);
        builder.Append(stats);
        
        var message = builder.ToString();
        var notification = task.OwnerMessageId.HasValue
            ? NotificationMessage.Edit(new(task.OwnerId, task.OwnerMessageId.Value), message)
            : NotificationMessage
                .Create(task.OwnerId, message)
                .AddHandler((c, p) => new AttachMessageCommand(
                    c,
                    task.Id,
                    int.Parse(p),
                    MessageType.Owner.ToString()));

        if (task.State == TaskForReviewState.OnCorrection)
        {
            var moveToNextRoundButton = await _messageBuilder.Build(Messages.Reviewer_MoveToNextRound, languageId);
            notification.WithButton(new Button(moveToNextRoundButton,
                $"{CommandList.MoveToNextRound}{task.Id:N}"));
        }

        return notification;
    }

    private async Task<NotificationMessage> ReviewFinish(TaskForReview task, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);

        var languageId = await _teamAccessor.GetClientLanguage(task.BotId, task.OwnerId, token);
        var totalTime = task.GetTotalTime(DateTimeOffset.UtcNow);
        var stateMessage = task.State == TaskForReviewState.AcceptWithComments
            ? Messages.Reviewer_AcceptedWithComments
            : Messages.Reviewer_Accepted;
        
        var builder = new StringBuilder();
        builder.AppendLine(await _messageBuilder.Build(stateMessage, languageId));
        builder.AppendLine();
        builder.AppendLine(task.Description);
        builder.AppendLine();
        builder.AppendLine(await _messageBuilder.Build(
            Messages.Reviewer_TotalTime,
            languageId,
            totalTime.ToString(GlobalSettings.TimeFormat)));

        return NotificationMessage.Create(task.OwnerId, builder.ToString());
    }
    
    private string StateAsIcon(TaskForReview task)
    {
        return task.State switch
        {
            TaskForReviewState.New => Icons.Waiting,
            TaskForReviewState.InProgress => Icons.InProgress,
            TaskForReviewState.OnCorrection => Icons.OnCorrection,
            TaskForReviewState.Accept => Icons.Accept,
            TaskForReviewState.AcceptWithComments => Icons.AcceptWithComments,
            _ => throw new ArgumentOutOfRangeException(nameof(task.State), task.State, "State out of range.")
        };
    }
}