using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress;

internal sealed class MoveToInProgressCommandHandler : IRequestHandler<MoveToInProgressCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly ReviewerOptions _options;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly TelegramBotClientProvider _telegramBotClientProvider;
    private readonly ITranslateProvider _translateProvider;

    public MoveToInProgressCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        ReviewerOptions options,
        IMessageBuilderService messageBuilderService,
        TelegramBotClientProvider telegramBotClientProvider,
        ITranslateProvider translateProvider)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _telegramBotClientProvider = telegramBotClientProvider ?? throw new ArgumentNullException(nameof(telegramBotClientProvider));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<CommandResult> Handle(MoveToInProgressCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var client = _telegramBotClientProvider.Get();
        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        
        if (taskForReview.CanMoveToInProgress())
        {
            taskForReview.MoveToInProgress(_options.Workday.NotificationInterval);

            if (taskForReview.MessageId.HasValue)
            {
                var newTaskForReview = await _messageBuilderService.NewTaskForReviewBuild(
                    command.MessageContext.LanguageId,
                    taskForReview,
                    token);
                await client.EditMessageTextAsync(
                    taskForReview.ChatId,
                    taskForReview.MessageId.Value,
                    newTaskForReview.Text,
                    entities: newTaskForReview.Entities,
                    cancellationToken: token);
            }

            await _taskForReviewRepository.Upsert(taskForReview, token);
            
            await client.SendTextMessageAsync(
                taskForReview.ReviewerId,
                await _translateProvider.Get(
                    Messages.Reviewer_OperationApplied,
                    command.MessageContext.LanguageId,
                    token),
                cancellationToken: token);
        }

        return CommandResult.Empty;
    }
}