using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Model.Commands.MoveToNextRound;
using MediatR;

namespace Inc.TeamAssistant.Reviewer.Application.CommandHandlers.MoveToNextRound;

internal sealed class MoveToNextRoundCommandHandler : IRequestHandler<MoveToNextRoundCommand, CommandResult>
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ITranslateProvider _translateProvider;
    private readonly ITeamAccessor _teamAccessor;

    public MoveToNextRoundCommandHandler(
        ITaskForReviewRepository taskForReviewRepository,
        IMessageBuilderService messageBuilderService,
        ITranslateProvider translateProvider,
        ITeamAccessor teamAccessor)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _translateProvider = translateProvider ?? throw new ArgumentNullException(nameof(translateProvider));
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

        taskForReview.MoveToNextRound();
        
        var notifications = new List<NotificationMessage>();

        if (taskForReview.MessageId.HasValue)
            notifications.Add(await _messageBuilderService.BuildMessageNewTaskForReview(
                taskForReview,
                reviewer,
                owner));

        var operationAppliedMessage = await _translateProvider.Get(
            Messages.Reviewer_OperationApplied,
            owner.GetLanguageId(),
            token);
        notifications.Add(NotificationMessage.Create(taskForReview.OwnerId, operationAppliedMessage));

        await _taskForReviewRepository.Upsert(taskForReview, token);
        
        return CommandResult.Build(notifications.ToArray());
    }
}