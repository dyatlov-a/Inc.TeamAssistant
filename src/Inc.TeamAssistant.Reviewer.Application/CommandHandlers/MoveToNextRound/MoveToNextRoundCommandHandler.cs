using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound;

internal sealed class MoveToNextRoundCommandHandler : IRequestHandler<MoveToNextRoundCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITeamAccessor _teamAccessor;

    public MoveToNextRoundCommandHandler(
        ITaskForReviewRepository repository,
        IReviewMessageBuilder reviewMessageBuilder,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<CommandResult> Handle(MoveToNextRoundCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await _repository.GetById(command.TaskId, token);
        if (!taskForReview.CanMoveToNextRound())
            return CommandResult.Empty;
        
        var reviewer = await _teamAccessor.EnsurePerson(taskForReview.ReviewerId, token);
        var owner = await _teamAccessor.EnsurePerson(taskForReview.OwnerId, token);

        await _repository.Upsert(taskForReview.MoveToNextRound(DateTimeOffset.UtcNow), token);
        
        var notifications = await _reviewMessageBuilder.Build(
            command.MessageContext.ChatMessage.MessageId,
            taskForReview,
            reviewer,
            owner,
            command.MessageContext.Bot,
            token);
        return CommandResult.Build(notifications.ToArray());
    }
}