using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress;

internal sealed class MoveToInProgressCommandHandler : IRequestHandler<MoveToInProgressCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly ReviewerOptions _options;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITranslateProvider _translateProvider;

    public MoveToInProgressCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        ReviewerOptions options,
        IMessageBuilderService messageBuilderService,
        ITranslateProvider translateProvider)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<CommandResult> Handle(MoveToInProgressCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);

        if (!taskForReview.CanMoveToInProgress())
            return CommandResult.Empty;

        taskForReview.MoveToInProgress(_options.Workday.NotificationInterval);
        
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
        notifications.Add(NotificationMessage.Create(taskForReview.ReviewerId, operationAppliedMessage));
        
        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}