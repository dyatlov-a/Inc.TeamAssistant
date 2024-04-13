using System.Text;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Languages;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class MessageBuilderService : IMessageBuilderService
{
    private readonly ITranslateProvider _translateProvider;
    private readonly ITeamAccessor _teamAccessor;

    public MessageBuilderService(ITranslateProvider translateProvider, ITeamAccessor teamAccessor)
    {
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
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
        var text = await _translateProvider.Get(
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
            TaskForReviewState.IsArchived => "👍",
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
                .AddHandler((c, p) => new AttachMessageCommand(c, taskForReview.Id, int.Parse(p)));

        return notification;
    }
    
    public async Task<NotificationMessage> BuildNeedReview(
        TaskForReview task,
        Person reviewer,
        bool? hasInProgressAction,
        ChatMessage? chatMessage,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);

        var languageId = await _teamAccessor.GetClientLanguage(reviewer.Id, token);
        var message = await _translateProvider.Get(Messages.Reviewer_NeedReview, languageId, task.Description);

        var notification = chatMessage is null
            ? NotificationMessage.Create(reviewer.Id, message)
            : NotificationMessage.Edit(chatMessage, message);

        if (hasInProgressAction.HasValue)
            foreach (var command in GetReviewerCommands(hasInProgressAction.Value))
            {
                var text = await _translateProvider.Get(command.MessageId, languageId);
                notification.WithButton(new Button(text, $"{command.Command}{task.Id:N}"));
            }

        return notification;
    }

    public async Task<NotificationMessage> BuildMoveToNextRound(
        TaskForReview task,
        Person owner,
        ChatMessage? chatMessage,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(owner);

        var languageId = await _teamAccessor.GetClientLanguage(owner.Id, token);
        var message = await _translateProvider.Get(Messages.Reviewer_ReviewDeclined, languageId, task.Description);

        if (chatMessage is not null)
            return NotificationMessage.Edit(chatMessage, message);

        var text = await _translateProvider.Get(Messages.Reviewer_MoveToNextRound, languageId);
        var notification = NotificationMessage
            .Create(owner.Id, message)
            .WithButton(new Button(text, $"{CommandList.MoveToNextRound}{task.Id:N}"));
        
        return notification;
    }

    public async Task<NotificationMessage> BuildReviewAccepted(
        TaskForReview task,
        Person owner,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(owner);
        
        var message = await _translateProvider.Get(
            Messages.Reviewer_Accepted,
            await _teamAccessor.GetClientLanguage(owner.Id, token),
            task.Description);
        var notification = NotificationMessage.Create(owner.Id, message);

        return notification;
    }
    
    private IEnumerable<(MessageId MessageId, string Command)> GetReviewerCommands(bool hasInProgressAction)
    {
        if (hasInProgressAction)
            yield return (Messages.Reviewer_MoveToInProgress, CommandList.MoveToInProgress);

        yield return (Messages.Reviewer_MoveToAccept, CommandList.Accept);
        yield return (Messages.Reviewer_MoveToDecline, CommandList.Decline);
    }
}