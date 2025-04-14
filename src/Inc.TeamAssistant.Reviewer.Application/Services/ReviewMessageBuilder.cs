using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Extensions;
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
    private readonly IReviewMetricsProvider _metricsProvider;
    private readonly ReviewTeamMetricsFactory _metricsFactory;
    private readonly DraftTaskForReviewService _draftService;

    public ReviewMessageBuilder(
        IMessageBuilder messageBuilder,
        ITeamAccessor teamAccessor,
        IReviewMetricsProvider metricsProvider,
        ReviewTeamMetricsFactory metricsFactory,
        DraftTaskForReviewService draftService)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
        _metricsFactory = metricsFactory ?? throw new ArgumentNullException(nameof(metricsFactory));
        _draftService = draftService ?? throw new ArgumentNullException(nameof(draftService));
    }

    public async Task<IReadOnlyCollection<NotificationMessage>> Build(
        int messageId,
        TaskForReview task,
        BotContext botContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(botContext);

        var hasOwnerAction = messageId == task.OwnerMessageId;
        var metricsByTeam = _metricsProvider.Get(task.TeamId);
        var metricsByTask = await _metricsFactory.Create(task, token);
        var owner = await _teamAccessor.EnsurePerson(task.OwnerId, token);
        var reviewer = await _teamAccessor.EnsurePerson(task.ReviewerId, token);

        var notifications = new List<NotificationMessage>
        {
            await MessageForTeam(task, reviewer, owner, metricsByTeam, metricsByTask, token),
            await MessageForReviewer(task, botContext, owner, metricsByTeam, metricsByTask, token)
        };

        if (!task.ReviewerMessageId.HasValue && task.OriginalReviewerMessageId.HasValue && task.HasReassign())
            notifications.Add(await HideControlsForOriginalReviewer(
                task.OriginalReviewerId!.Value,
                task.OriginalReviewerMessageId.Value,
                task,
                token));
        
        if (task.State == TaskForReviewState.OnCorrection || hasOwnerAction)
            notifications.Add(await MessageForOwner(task, reviewer, metricsByTeam, metricsByTask, token));
        
        if (task.State == TaskForReviewState.Accept)
            notifications.Add(await ReviewFinish(task, reviewer, token));

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
        var moveToReviewText = _messageBuilder.Build(Messages.Reviewer_PreviewMoveToReview, languageId);
        var moveToReviewCommand = $"{CommandList.MoveToReview}{draft.Id:N}";
        var removeDraftText = _messageBuilder.Build(Messages.Reviewer_PreviewRemoveDraft, languageId);
        var removeDraftCommand = $"{CommandList.RemoveDraft}{draft.Id:N}";
        var builder = NotificationBuilder.Create()
            .Add(sb => sb
                .AppendLine(_messageBuilder.Build(Messages.Reviewer_PreviewTitle, languageId))
                .AppendLine()
                .AppendLine(draft.Description)
                .AppendLine());

        if (draft.TargetPersonId.HasValue)
        {
            var reviewTarget = await _teamAccessor.EnsurePerson(draft.TargetPersonId.Value, token);
            builder.Add(sb => sb.AppendLine(ReviewTargetMessage(reviewTarget.DisplayName, languageId)));

            if (!await _draftService.HasTeammate(draft.TeamId, draft.TargetPersonId.Value, token))
                builder.Add(sb => sb
                    .AppendLine()
                    .Append(GlobalResources.Icons.Alert)
                    .Append(_messageBuilder.Build(
                        Messages.Reviewer_PreviewCheckTeammate,
                        languageId,
                        reviewTarget.DisplayName,
                        teamContext.Name))
                    .AppendLine());
        }
        else
            builder.Add(sb => sb.AppendLine(ReviewTargetMessage(teamContext.Name, languageId)));

        if (!_draftService.HasDescriptionAndLinks(draft.Description))
            builder.Add(sb => sb
                .AppendLine()
                .Append(GlobalResources.Icons.Alert)
                .Append(_messageBuilder.Build(Messages.Reviewer_PreviewCheckDescription, languageId))
                .AppendLine());

        var notification = builder
            .Add(sb => sb
                .AppendLine()
                .AppendLine(_messageBuilder.Build(Messages.Reviewer_PreviewEditHelp, languageId)))
            .Build(m => draft.PreviewMessageId.HasValue
                ? NotificationMessage.Edit(new ChatMessage(draft.ChatId, draft.PreviewMessageId.Value), m)
                : NotificationMessage.Create(draft.ChatId, m)
                    .ReplyTo(draft.MessageId)
                    .WithHandler((c, p) => new AttachPreviewCommand(c, draft.Id, int.Parse(p))))
            .WithButton(new Button(moveToReviewText, moveToReviewCommand))
            .WithButton(new Button(removeDraftText, removeDraftCommand));

        return notification;
    }

    private string ReviewTargetMessage(string personName, LanguageId languageId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(personName);
        ArgumentNullException.ThrowIfNull(languageId);
        
        var reviewTargetMessageTemplate = _messageBuilder.Build(
            Messages.Reviewer_PreviewReviewerTemplate,
            languageId);
        var reviewTargetMessage = string.Format(reviewTargetMessageTemplate, personName);
        
        return reviewTargetMessage;
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
        var message = _messageBuilder.Build(messageTextId, languageId);
        var totalTimeMessage = _messageBuilder.Build(Messages.Reviewer_TotalTime, languageId, workTimeTotal.ToTime());
        var notification = NotificationBuilder.Create()
            .Add(sb => sb.AppendLine(message).AppendLine().AppendLine(totalTimeMessage))
            .Build(m => NotificationMessage.Create(personId, m).ReplyTo(messageId));

        return notification;
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
        
        var ownerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, owner.Id, token);
        var reviewerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, reviewer.Id, token);
        
        Func<NotificationMessage, NotificationMessage> attachPersons = n => n;

        var notification = NotificationBuilder.Create()
            .Add(sb => sb
                .AppendLine(_messageBuilder.Build(Messages.Reviewer_NewTaskForReview, ownerLanguageId))
                .AppendLine(_messageBuilder.Build(Messages.Reviewer_Owner, ownerLanguageId, owner.DisplayName))
                .Append(_messageBuilder.Build(task.GetReviewerMessageId(), ownerLanguageId)))
            .Add(sb => reviewer
                .AddTo(sb, (p, o) => attachPersons += n => n.AttachPerson(p, reviewerLanguageId, o))
                .AppendLine())
            .Add(sb => sb.AppendLine().AppendLine(task.Description).AppendLine(task.AsIcon()))
            .Add(sb => sb.Append(ReviewStatsBuilder
                .Create(_messageBuilder)
                .WithReviewMetrics()
                .WithCorrectionMetrics()
                .Build(task, metricsByTeam, metricsByTask, ownerLanguageId)))
            .Build(m => task.MessageId.HasValue
                ? NotificationMessage.Edit(new ChatMessage(task.ChatId, task.MessageId.Value), m)
                : NotificationMessage
                    .Create(task.ChatId, m)
                    .WithHandler((c, p) => new AttachMessageCommand(
                        c,
                        task.Id,
                        int.Parse(p),
                        MessageType.Shared.ToString())));

        var result = attachPersons(notification);
        return result;
    }

    private async Task<NotificationMessage> HideControlsForOriginalReviewer(
        long reviewerId,
        int messageId,
        TaskForReview task,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);

        var languageId = await _teamAccessor.GetClientLanguage(task.BotId, reviewerId, token);
        var notification = NotificationBuilder.Create()
            .Add(sb => sb
                .AppendLine(_messageBuilder.Build(Messages.Reviewer_NeedReview, languageId))
                .AppendLine()
                .AppendLine(task.Description))
            .Build(m => NotificationMessage.Edit(new(reviewerId, messageId), m));
        
        return notification;
    }

    private async Task<NotificationMessage> MessageForReviewer(
        TaskForReview task,
        BotContext botContext,
        Person owner,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(botContext);
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);
        
        var ownerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, owner.Id, token);
        var reviewerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, task.ReviewerId, token);
        var notification = NotificationBuilder.Create()
            .Add(sb => sb
                .AppendLine(_messageBuilder.Build(Messages.Reviewer_NeedReview, reviewerLanguageId))
                .AppendLine(_messageBuilder.Build(Messages.Reviewer_Owner, ownerLanguageId, owner.DisplayName))
                .AppendLine()
                .AppendLine(task.Description))
            .Add(sb => sb
                .Append(ReviewStatsBuilder
                    .Create(_messageBuilder)
                    .WithReviewMetrics()
                    .Build(task, metricsByTeam, metricsByTask, reviewerLanguageId)))
            .Build(m => task.ReviewerMessageId.HasValue
                ? NotificationMessage.Edit(new(task.ReviewerId, task.ReviewerMessageId.Value), m)
                : NotificationMessage
                    .Create(task.ReviewerId, m)
                    .WithHandler((c, p) => new AttachMessageCommand(
                        c,
                        task.Id,
                        int.Parse(p),
                        MessageType.Reviewer.ToString())))
            .AddIf(task.State == TaskForReviewState.New, n =>
            {
                var inProgressText = _messageBuilder.Build(Messages.Reviewer_MoveToInProgress, reviewerLanguageId);
                var inProgressCommand = $"{CommandList.MoveToInProgress}{task.Id:N}";
                n.WithButton(new Button(inProgressText, inProgressCommand));

                if (!task.HasReassign())
                {
                    var reassignReviewText = _messageBuilder.Build(Messages.Reviewer_Reassign, reviewerLanguageId);
                    var reassignReviewCommand = $"{CommandList.ReassignReview}{task.Id:N}";
                    n.WithButton(new Button(reassignReviewText, reassignReviewCommand));
                }
            })
            .AddIf(task.State == TaskForReviewState.InProgress, n =>
            {
                var moveToAcceptText = _messageBuilder.Build(Messages.Reviewer_MoveToAccept, reviewerLanguageId);
                var moveToAcceptCommand = $"{CommandList.Accept}{task.Id:N}";
                n.WithButton(new Button(moveToAcceptText, moveToAcceptCommand));

                if (botContext.CanAcceptWithComments())
                {
                    var moveToAcceptWithCommentsText = _messageBuilder.Build(
                        Messages.Reviewer_MoveToAcceptWithComments,
                        reviewerLanguageId);
                    var moveToAcceptWithCommentsCommand = $"{CommandList.AcceptWithComments}{task.Id:N}";
                    n.WithButton(new Button(moveToAcceptWithCommentsText, moveToAcceptWithCommentsCommand));
                }

                var moveToDeclineText = _messageBuilder.Build(Messages.Reviewer_MoveToDecline, reviewerLanguageId);
                var moveToDeclineCommand = $"{CommandList.Decline}{task.Id:N}";
                n.WithButton(new Button(moveToDeclineText, moveToDeclineCommand));
            });

        return notification;
    }

    private async Task<NotificationMessage> MessageForOwner(
        TaskForReview task,
        Person reviewer,
        ReviewTeamMetrics metricsByTeam,
        ReviewTeamMetrics metricsByTask,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);
        ArgumentNullException.ThrowIfNull(metricsByTeam);
        ArgumentNullException.ThrowIfNull(metricsByTask);

        var ownerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, task.OwnerId, token);
        var notification = NotificationBuilder.Create()
            .Add(sb => sb.AppendLine(_messageBuilder.Build(Messages.Reviewer_ReviewDeclined, ownerLanguageId))
                .Append(_messageBuilder.Build(task.GetReviewerMessageId(), ownerLanguageId))
                .Append($" {reviewer.DisplayName}")
                .AppendLine()
                .AppendLine()
                .AppendLine(task.Description))
            .Add(sb => sb
                .Append(ReviewStatsBuilder
                    .Create(_messageBuilder)
                    .WithCorrectionMetrics()
                    .Build(task, metricsByTeam, metricsByTask, ownerLanguageId)))
            .Build(m => task.OwnerMessageId.HasValue
                ? NotificationMessage.Edit(new(task.OwnerId, task.OwnerMessageId.Value), m)
                : NotificationMessage
                    .Create(task.OwnerId, m)
                    .WithHandler((c, p) => new AttachMessageCommand(
                        c,
                        task.Id,
                        int.Parse(p),
                        MessageType.Owner.ToString())))
            .AddIf(task.State == TaskForReviewState.OnCorrection, n =>
            {
                var moveToNextRoundText = _messageBuilder.Build(Messages.Reviewer_MoveToNextRound, ownerLanguageId);
                var moveToNextRoundCommand = $"{CommandList.MoveToNextRound}{task.Id:N}";
                n.WithButton(new Button(moveToNextRoundText, moveToNextRoundCommand));
            });

        return notification;
    }

    private async Task<NotificationMessage> ReviewFinish(TaskForReview task, Person reviewer, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);

        var ownerLanguageId = await _teamAccessor.GetClientLanguage(task.BotId, task.OwnerId, token);
        var totalTime = task.GetTotalTime(DateTimeOffset.UtcNow);
        var stateMessage = task.State == TaskForReviewState.AcceptWithComments
            ? Messages.Reviewer_AcceptedWithComments
            : Messages.Reviewer_Accepted;
        var totalTimeMessage = _messageBuilder.Build(Messages.Reviewer_TotalTime, ownerLanguageId, totalTime.ToTime());
        var notification = NotificationBuilder.Create()
            .Add(sb => sb
                .AppendLine(_messageBuilder.Build(stateMessage, ownerLanguageId))
                .Append(_messageBuilder.Build(task.GetReviewerMessageId(), ownerLanguageId))
                .Append($" {reviewer.DisplayName}")
                .AppendLine()
                .AppendLine()
                .AppendLine(task.Description)
                .AppendLine()
                .AppendLine(totalTimeMessage))
            .Build(m => NotificationMessage.Create(task.OwnerId, m));

        return notification;
    }
}