using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Commands;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Model.Commands.SendNotification;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.SendNotification;

internal sealed class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _repository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ReviewerOptions _options;
    private readonly ITeamAccessor _teamAccessor;

    public SendNotificationCommandHandler(
        ITaskForReviewRepository repository,
        IMessageBuilderService messageBuilderService,
        ReviewerOptions options,
        ITeamAccessor teamAccessor)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _messageBuilderService = messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
    }

    public async Task<CommandResult> Handle(SendNotificationCommand command, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(command);

        var taskForReview = await _repository.GetById(command.TaskId, token);
        
        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);

        taskForReview.SetNextNotificationTime(DateTimeOffset.UtcNow, _options.NotificationInterval);

        var notifications = taskForReview.State switch
        {
            TaskForReviewState.New => [await _messageBuilderService.BuildNeedReview(
                taskForReview,
                reviewer,
                hasInProgressAction: true,
                chatMessage: null,
                token)],
            TaskForReviewState.InProgress => [await _messageBuilderService.BuildNeedReview(
                taskForReview,
                reviewer,
                hasInProgressAction: false,
                chatMessage: null,
                token)],
            TaskForReviewState.OnCorrection => [await _messageBuilderService.BuildMoveToNextRound(
                taskForReview,
                owner,
                chatMessage: null,
                token)],
            _ => Array.Empty<NotificationMessage>()
        };

        await _repository.Upsert(taskForReview, token);

        return CommandResult.Build(notifications);
    }
}