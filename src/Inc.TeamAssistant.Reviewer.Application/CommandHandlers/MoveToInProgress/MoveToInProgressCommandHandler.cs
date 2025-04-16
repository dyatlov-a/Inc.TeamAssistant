using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress;

internal sealed class MoveToInProgressCommandHandler : IRequestHandler<MoveToInProgressCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;

    public MoveToInProgressCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
    }

    public async Task<CommandResult> Handle(MoveToInProgressCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await command.TaskId.Required(_repository.Find, token);
        if (!taskForReview.CanMoveToInProgress())
            return CommandResult.Empty;
        
        await _repository.Upsert(
            taskForReview.MoveToInProgress(DateTimeOffset.UtcNow, command.MessageContext.Bot.GetNotificationIntervals()),
            token);
        
        var notifications = await _reviewMessageBuilder.Build(
            command.MessageContext.ChatMessage.MessageId,
            taskForReview,
            command.MessageContext.Bot,
            token);
        return CommandResult.Build(notifications.ToArray());
    }
}