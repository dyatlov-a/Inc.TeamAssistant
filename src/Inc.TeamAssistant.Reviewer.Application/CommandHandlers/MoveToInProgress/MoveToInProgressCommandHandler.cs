using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToInProgress;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToInProgress;

internal sealed class MoveToInProgressCommandHandler : IRequestHandler<MoveToInProgressCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IBotAccessor _botAccessor;

    public MoveToInProgressCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IReviewMessageBuilder reviewMessageBuilder,
        ITeamAccessor teamAccessor,
        IBotAccessor botAccessor)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _reviewMessageBuilder =
            reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _botAccessor = botAccessor ?? throw new ArgumentNullException(nameof(botAccessor));
    }

    public async Task<CommandResult> Handle(MoveToInProgressCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        var botContext = await _botAccessor.GetBotContext(taskForReview.BotId, token);

        if (!taskForReview.CanMoveToInProgress())
            return CommandResult.Empty;
        
        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);

        taskForReview.MoveToInProgress(DateTimeOffset.UtcNow, botContext.GetNotificationIntervals());

        var notifications = await _reviewMessageBuilder.Build(
            command.MessageContext.ChatMessage.MessageId,
            taskForReview,
            reviewer,
            owner,
            command.MessageContext.Bot,
            token);
        
        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}