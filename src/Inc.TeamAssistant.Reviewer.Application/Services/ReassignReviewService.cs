using Inc.TeamAssistant.Holidays.Extensions;
using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReassignReviewService
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly ITaskForReviewReader _taskForReviewReader;
    
    public ReassignReviewService(
        ITaskForReviewRepository taskForReviewRepository,
        ITeamAccessor teamAccessor,
        IReviewMessageBuilder reviewMessageBuilder,
        ITaskForReviewReader taskForReviewReader)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _reviewMessageBuilder =
            reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _taskForReviewReader = taskForReviewReader ?? throw new ArgumentNullException(nameof(taskForReviewReader));
    }
    
    public async Task<IReadOnlyCollection<NotificationMessage>> ReassignReview(
        int messageId,
        Guid taskId,
        CancellationToken token)
    {
        var task = await _taskForReviewRepository.GetById(taskId, token);
        if (!task.CanAccept())
            return Array.Empty<NotificationMessage>();
        
        var history = await _taskForReviewReader.GetHistory(
            task.TeamId,
            DateTimeOffset.UtcNow.GetLastDayOfWeek(DayOfWeek.Monday),
            token);
        var teammates = await _teamAccessor.GetTeammates(task.TeamId, token);
        var lastReviewerId = await _taskForReviewRepository.FindLastReviewer(task.TeamId, task.OwnerId, token);

        task.Reassign(
            DateTimeOffset.UtcNow,
            teammates.Select(t => t.Id).ToArray(),
            history,
            task.ReviewerId,
            lastReviewerId);
        
        var owner = await _teamAccessor.FindPerson(task.OwnerId, token);
        if (owner is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.OwnerId);
        var newReviewer = await _teamAccessor.FindPerson(task.ReviewerId, token);
        if (newReviewer is null)
            throw new TeamAssistantUserException(Messages.Connector_PersonNotFound, task.ReviewerId);

        var notifications = await _reviewMessageBuilder.Build(
            messageId,
            task,
            newReviewer,
            owner,
            token);

        await _taskForReviewRepository.Upsert(task, token);

        return notifications;
    }
}