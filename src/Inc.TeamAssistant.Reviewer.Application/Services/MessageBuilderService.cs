using System.Text;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.AttachMessage;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class MessageBuilderService : IMessageBuilderService
{
    private static readonly IReadOnlyCollection<(MessageId MessageId, string Command)> ReviewerCommands = new[]
    {
        (Messages.Reviewer_MoveToInProgress, CommandList.MoveToInProgress),
        (Messages.Reviewer_MoveToAccept, CommandList.Accept),
        (Messages.Reviewer_MoveToDecline, CommandList.Decline)
    };
    
    private readonly ITranslateProvider _translateProvider;

    public MessageBuilderService(ITranslateProvider translateProvider)
    {
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<NotificationMessage> BuildMessageNewTaskForReview(
        TaskForReview taskForReview,
        Person reviewer,
        Person owner)
    {
        ArgumentNullException.ThrowIfNull(taskForReview);
        ArgumentNullException.ThrowIfNull(reviewer);
        ArgumentNullException.ThrowIfNull(owner);

        var languageId = owner.GetLanguageId();
        var hasUsername = !string.IsNullOrWhiteSpace(reviewer.Username);
        var attachedPersonId = hasUsername ? null : (long?)reviewer.Id;
        var messageText = await _translateProvider.Get(
            Messages.Reviewer_NewTaskForReview,
            languageId,
            taskForReview.Description,
            owner.DisplayName,
            hasUsername ? $"@{reviewer.Username}" : reviewer.Name);
        var messageBuilder = new StringBuilder();
        var state = taskForReview.State switch
        {
            TaskForReviewState.New => "⏳",
            TaskForReviewState.InProgress => "🤩",
            TaskForReviewState.OnCorrection => "😱",
            TaskForReviewState.IsArchived => "👍",
            _ => throw new ArgumentOutOfRangeException($"State {taskForReview.State} out of range for {nameof(TaskForReviewState)}.")
        };
        
        messageBuilder.AppendLine(messageText);
        messageBuilder.AppendLine(state);
        
        var notification = taskForReview.MessageId.HasValue
            ? NotificationMessage.Edit(
                    new ChatMessage(taskForReview.ChatId, taskForReview.MessageId.Value),
                    messageBuilder.ToString())
                .AttachPerson(attachedPersonId)
            : NotificationMessage
                .Create(taskForReview.ChatId, messageBuilder.ToString())
                .AttachPerson(attachedPersonId)
                .AddHandler((c, p) => new AttachMessageCommand(c, taskForReview.Id, int.Parse(p)));

        return notification;
    }
    
    public async Task<NotificationMessage> BuildMessageNeedReview(TaskForReview task, Person reviewer)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(reviewer);

        var languageId = reviewer.GetLanguageId();
        var message = NotificationMessage.Create(
            reviewer.Id,
            await _translateProvider.Get(Messages.Reviewer_NeedReview, languageId, task.Description));

        foreach (var command in ReviewerCommands)
        {
            var text = await _translateProvider.Get(command.MessageId, languageId);
            message.WithButton(new Button(text, $"{command.Command}{task.Id:N}"));
        }

        return message;
    }

    public async Task<NotificationMessage> BuildMessageMoveToNextRound(TaskForReview task, Person owner)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(owner);

        var languageId = owner.GetLanguageId();
        var message = NotificationMessage.Create(
            owner.Id,
            await _translateProvider.Get(Messages.Reviewer_ReviewDeclined, languageId, task.Description));
        message.WithButton(new Button(
            await _translateProvider.Get(Messages.Reviewer_MoveToNextRound, languageId),
            $"{CommandList.MoveToNextRound}{task.Id:N}"));

        return message;
    }
}