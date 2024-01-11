using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Application.Services;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept;

internal sealed class MoveToAcceptCommandHandler : IRequestHandler<MoveToAcceptCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly TelegramBotClientProvider _telegramBotClientProvider;
    private readonly ITranslateProvider _translateProvider;

    public MoveToAcceptCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        TelegramBotClientProvider telegramBotClientProvider,
        ITranslateProvider translateProvider)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _telegramBotClientProvider = telegramBotClientProvider ?? throw new ArgumentNullException(nameof(telegramBotClientProvider));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task<CommandResult> Handle(MoveToAcceptCommand command, CancellationToken token)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));

        var client = _telegramBotClientProvider.Get();
        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        if (taskForReview.CanAccept())
        {
            taskForReview.Accept();

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
                
            await client.SendTextMessageAsync(
                taskForReview.OwnerId,
                await _translateProvider.Get(
                    Messages.Reviewer_Accepted,
                    command.MessageContext.LanguageId,
                    taskForReview.Description),
                cancellationToken: token);
            await client.SendTextMessageAsync(
                taskForReview.ReviewerId,
                await _translateProvider.Get(
                    Messages.Reviewer_OperationApplied,
                    command.MessageContext.LanguageId,
                    token),
                cancellationToken: token);

            await _taskForReviewRepository.Upsert(taskForReview, token);
        }

        return CommandResult.Empty;
    }
}