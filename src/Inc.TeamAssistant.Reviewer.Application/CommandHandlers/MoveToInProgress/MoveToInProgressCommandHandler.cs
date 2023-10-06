using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress;

internal sealed class MoveToInProgressCommandHandler : IRequestHandler<MoveToInProgressCommand>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly ReviewerOptions _options;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly TelegramBotClient _client;
    private readonly ITranslateProvider _translateProvider;

    public MoveToInProgressCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        ReviewerOptions options,
        IMessageBuilderService messageBuilderService,
        TelegramBotClient client,
        ITranslateProvider translateProvider)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task Handle(MoveToInProgressCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, cancellationToken);
        
        if (taskForReview.CanMoveToInProgress())
        {
            taskForReview.MoveToInProgress(_options.Workday.NotificationInterval);

            if (taskForReview.MessageId.HasValue)
            {
                var newTaskForReview = await _messageBuilderService.NewTaskForReviewBuild
                    (command.PersonLanguageId,
                        taskForReview);
                await _client.EditMessageTextAsync(
                    taskForReview.ChatId,
                    taskForReview.MessageId.Value,
                    newTaskForReview.Text,
                    entities: newTaskForReview.Entities,
                    cancellationToken: cancellationToken);
            }

            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);
            
            await _client.SendTextMessageAsync(
                taskForReview.Reviewer.Id,
                await _translateProvider.Get(
                    Messages.Reviewer_OperationApplied,
                    taskForReview.Reviewer.LanguageId,
                    cancellationToken),
                cancellationToken: cancellationToken);
        }
    }
}