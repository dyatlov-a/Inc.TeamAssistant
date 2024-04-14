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
    
    public ReassignReviewService(
        ITaskForReviewRepository taskForReviewRepository,
        ITeamAccessor teamAccessor,
        IMessageBuilderService messageBuilderService)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _messageBuilderService =
            messageBuilderService ?? throw new ArgumentNullException(nameof(messageBuilderService));
    }
    
    public async Task<IEnumerable<NotificationMessage>> ReassignReview(Guid taskId, CancellationToken token)
    {
        var taskForReview = await _taskForReviewRepository.GetById(taskId, token);
        if (!taskForReview.CanAccept())
            return Array.Empty<NotificationMessage>();
        
        var reviewer = await _teamAccessor.FindPerson(taskForReview.ReviewerId, token);
        if (reviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.ReviewerId);
        
        var owner = await _teamAccessor.FindPerson(taskForReview.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, taskForReview.OwnerId);
        
        var teammates = await _teamAccessor.GetTeammates(taskForReview.TeamId, token);
        var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(
            taskForReview.TeamId,
            taskForReview.ReviewerId,
            token);

        taskForReview
            .DetectReviewer(
                teammates.Select(t => t.Id).ToArray(),
                lastReviewerId,
                taskForReview.ReviewerId)
            .MoveToNextRound();
        
        var taskForReviewMessage = await _messageBuilderService.BuildNewTaskForReview(
            taskForReview,
            reviewer,
            owner,
            token);
        
        await _taskForReviewRepository.Upsert(taskForReview, token);

        return [taskForReviewMessage];
    }
}