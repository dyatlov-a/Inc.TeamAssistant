using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound;

internal sealed class MoveToNextRoundCommandHandler : IRequestHandler<MoveToNextRoundCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITeamAccessor _teamAccessor;

    public MoveToNextRoundCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        ITeamAccessor teamAccessor)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<CommandResult> Handle(MoveToNextRoundCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await _taskForReviewRepository.GetById(command.TaskId, token);
        if (!taskForReview.CanMoveToNextRound())
            return CommandResult.Empty;
        
        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);

        taskForReview.MoveToNextRound(DateTimeOffset.UtcNow);
        
        var notifications = new List<NotificationMessage>
        {
            await _messageBuilderService.BuildNewTaskForReview(taskForReview, reviewer, owner, token)
        };
        
        await foreach(var item in _messageBuilderService.BuildMoveToNextRound(taskForReview, owner, isPush: false, hasActions: false, token))
            notifications.Add(item);

        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}