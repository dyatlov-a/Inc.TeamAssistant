using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToDecline;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToDecline;

internal sealed class MoveToDeclineCommandHandler : IRequestHandler<MoveToDeclineCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;

    public MoveToDeclineCommandHandler(ITaskForReviewRepository repository, IReviewMessageBuilder reviewMessageBuilder)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
    }

    public async Task<CommandResult> Handle(MoveToDeclineCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await command.TaskId.Required(_repository.Find, token);
        if (!taskForReview.CanAccept())
            return CommandResult.Empty;
        
        await _repository.Upsert(
            taskForReview.Decline(DateTimeOffset.UtcNow, command.MessageContext.Bot.GetNotificationIntervals()),
            token);
        
        var notifications = await _reviewMessageBuilder.Build(taskForReview, fromOwner: false, token);
        return CommandResult.Build(notifications.ToArray());
    }
}