using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.SendPush;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.SendPush;

internal sealed class SendPushCommandHandler : IRequestHandler<SendPushCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly IBotAccessor _botAccessor;

    public SendPushCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder,
        IBotAccessor botAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task<CommandResult> Handle(SendPushCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var taskForReview = await _repository.GetById(command.TaskId, token);
        var botContext = await _botAccessor.GetBotContext(taskForReview.BotId, token);

        taskForReview.SetNextNotificationTime(DateTimeOffset.UtcNow, botContext.GetNotificationIntervals());

        var notification = TaskForReviewStateRules.ActiveStates.Contains(taskForReview.State)
            ? await _reviewMessageBuilder.Push(taskForReview, token)
            : null;

        await _repository.Upsert(taskForReview, token);

        return notification is null ? CommandResult.Empty : CommandResult.Build(notification);
    }
}