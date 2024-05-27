using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReassignReviewService
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IMessageBuilderService _messageBuilderService;
    private readonly ReviewHistoryService _reviewHistoryService;
    
    public ReassignReviewService(
        ITaskForReviewRepository taskForReviewRepository,
        ITeamAccessor teamAccessor,
        IMessageBuilderService messageBuilderService,
        ReviewHistoryService reviewHistoryService)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
        _reviewHistoryService = reviewHistoryService ?? throw new ArgumentNullException(nameof(reviewHistoryService));
    }
    
    public async Task<IEnumerable<NotificationMessage>> ReassignReview(Guid taskId, CancellationToken token)
    {
        var task = await _taskForReviewRepository.GetById(taskId, token);
        if (!task.CanAccept())
            return Array.Empty<NotificationMessage>();
        
        var reviewer = await _teamAccessor.FindPerson(task.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(task.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.OwnerId);
        
        var history = await _reviewHistoryService.GetHistory(task.TeamId, token);
        var teammates = await _teamAccessor.GetTeammates(task.TeamId, token);
        var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(task.TeamId, task.OwnerId, token);

        task.Reassign(
            DateTimeOffset.UtcNow,
            teammates.Select(t => t.Id).ToArray(),
            history,
            task.ReviewerId,
            lastReviewerId);
        
        var newReviewer = await _teamAccessor.FindPerson(task.ReviewerId, token);
        if (newReviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.ReviewerId);

        var notifications = new List<NotificationMessage>
        {
            await _messageBuilderService.BuildNewTaskForReview(
                task,
                newReviewer,
                owner,
                token)
        };

        await foreach (var item in _messageBuilderService.BuildMoveToInProgress(task, reviewer, isPush: false, token))
            notifications.Add(item);

        await _taskForReviewRepository.Upsert(task, token);

        return notifications;
    }
}