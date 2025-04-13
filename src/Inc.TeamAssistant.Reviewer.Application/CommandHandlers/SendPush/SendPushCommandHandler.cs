using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.SendPush;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.SendPush;

internal sealed class SendPushCommandHandler : IRequestHandler<SendPushCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;

    public SendPushCommandHandler(ITaskForReviewRepository repository, IReviewMessageBuilder reviewMessageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
    }

    public async Task<CommandResult> Handle(SendPushCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);
        
        var botContext = command.MessageContext.Bot;
        var taskForReview = await command.TaskId.Required(_repository.Find, token);

        await _repository.Upsert(
            taskForReview.SetNextNotificationTime(DateTimeOffset.UtcNow, botContext.GetNotificationIntervals()),
            token);

        var notification = TaskForReviewStateRules.ActiveStates.Contains(taskForReview.State)
            ? await _reviewMessageBuilder.Push(taskForReview, token)
            : null;
        return notification is null
            ? CommandResult.Empty
            : CommandResult.Build(notification);
    }
}