using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound;

internal sealed class MoveToNextRoundCommandHandler : IRequestHandler<MoveToNextRoundCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITranslateProvider _translateProvider;

    public MoveToNextRoundCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        ITranslateProvider translateProvider)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<CommandResult> Handle(MoveToNextRoundCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        if (!taskForReview.CanMoveToNextRound())
            return CommandResult.Empty;

        taskForReview.MoveToNextRound();
        
        var notifications = new List<NotificationMessage>();

        if (taskForReview.MessageId.HasValue)
        {
            var taskForReviewMessage = await _messageBuilderService.NewTaskForReviewBuild(
                command.MessageContext.LanguageId,
                taskForReview,
                token);
            notifications.Add(NotificationMessage.Edit(
                    new ChatMessage(taskForReview.ChatId, taskForReview.MessageId.Value),
                    taskForReviewMessage.Text)
                .AttachPerson(taskForReviewMessage.AttachedPersonId));
        }

        var operationAppliedMessage = await _translateProvider.Get(
            Messages.Reviewer_OperationApplied,
            command.MessageContext.LanguageId,
            token);
        notifications.Add(NotificationMessage.Create(taskForReview.OwnerId, operationAppliedMessage));

        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}