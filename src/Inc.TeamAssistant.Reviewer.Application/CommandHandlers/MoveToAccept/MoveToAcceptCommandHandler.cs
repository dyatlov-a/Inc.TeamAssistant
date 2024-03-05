using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept;

internal sealed class MoveToAcceptCommandHandler : IRequestHandler<MoveToAcceptCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITranslateProvider _translateProvider;

    public MoveToAcceptCommandHandler(
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

    public async Task<CommandResult> Handle(MoveToAcceptCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        if (!taskForReview.CanAccept())
            return CommandResult.Empty;

        taskForReview.Accept();
        
        var notifications = new List<NotificationMessage>();

        if (taskForReview.MessageId.HasValue)
        {
            var taskForReviewMessage = await _messageBuilderService.NewTaskForReviewBuild(
                command.MessageContext.LanguageId,
                taskForReview,
                token);
            notifications.Add(NotificationMessage.Edit(
                new ChatMessage(taskForReview.ChatId, taskForReview.MessageId.Value),
                taskForReviewMessage));
        }

        var reviewerAcceptedMessage = await _translateProvider.Get(
            Messages.Reviewer_Accepted,
            command.MessageContext.LanguageId,
            taskForReview.Description);
        notifications.Add(NotificationMessage.Create(taskForReview.OwnerId, reviewerAcceptedMessage));

        var operationAppliedMessage = await _translateProvider.Get(
            Messages.Reviewer_OperationApplied,
            command.MessageContext.LanguageId);
        notifications.Add(NotificationMessage.Create(taskForReview.ReviewerId, operationAppliedMessage));

        await _taskForReviewRepository.Upsert(taskForReview, token);

        return CommandResult.Build(notifications.ToArray());
    }
}