using Inc.TeamAssistant.Languages;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;
using Telegram.Bot;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept;

internal sealed class MoveToAcceptCommandHandler : IRequestHandler<MoveToAcceptCommand>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITelegramBotClient _client;
    private readonly ITranslateProvider _translateProvider;

    public MoveToAcceptCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        ITelegramBotClient client,
        ITranslateProvider translateProvider)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
    }

    public async Task Handle(MoveToAcceptCommand command, CancellationToken cancellationToken)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        
        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, cancellationToken);
        if (taskForReview.CanAccept())
        {
            taskForReview.Accept();

            if (taskForReview.MessageId.HasValue)
            {
                var newTaskForReview = await _messageBuilderService.NewTaskForReviewBuild(
                    command.PersonLanguageId,
                    taskForReview);
                await _client.EditMessageTextAsync(
                    taskForReview.ChatId,
                    taskForReview.MessageId.Value,
                    newTaskForReview.Text,
                    entities: newTaskForReview.Entities,
                    cancellationToken: cancellationToken);
            }
                
            await _client.SendTextMessageAsync(
                taskForReview.Owner.Id,
                await _translateProvider.Get(
                    Messages.Reviewer_Accepted,
                    taskForReview.Owner.LanguageId,
                    taskForReview.Description),
                cancellationToken: cancellationToken);
            await _client.SendTextMessageAsync(
                taskForReview.Reviewer.Id,
                await _translateProvider.Get(
                    Messages.Reviewer_OperationApplied,
                    taskForReview.Reviewer.LanguageId,
                    cancellationToken),
                cancellationToken: cancellationToken);

            await _taskForReviewRepository.Upsert(taskForReview, cancellationToken);
        }
    }
}