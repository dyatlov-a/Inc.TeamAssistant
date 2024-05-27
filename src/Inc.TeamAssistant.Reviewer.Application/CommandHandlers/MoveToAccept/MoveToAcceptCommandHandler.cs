using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToAccept;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToAccept;

internal sealed class MoveToAcceptCommandHandler : IRequestHandler<MoveToAcceptCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITeamAccessor _teamAccessor;

    public MoveToAcceptCommandHandler(
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

    public async Task<CommandResult> Handle(MoveToAcceptCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var task = await _taskForReviewRepository.GetById(command.TaskId, token);
        if (!task.CanAccept())
            return CommandResult.Empty;

        var reviewer = await _teamAccessor.FindPerson(task.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(task.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.OwnerId);

        task.Accept(DateTimeOffset.UtcNow);
        
        var notifications = new List<NotificationMessage>
        {
            await _messageBuilderService.BuildNewTaskForReview(task, reviewer, owner, token),
            await _messageBuilderService.BuildReviewAccepted(task, owner, token)
        };
        
        await foreach(var item in _messageBuilderService.BuildMoveToReviewActions(task, reviewer, isPush: false, hasActions: false, token))
            notifications.Add(item);

        await _taskForReviewRepository.Upsert(task, token);

        return CommandResult.Build(notifications.ToArray());
    }
}