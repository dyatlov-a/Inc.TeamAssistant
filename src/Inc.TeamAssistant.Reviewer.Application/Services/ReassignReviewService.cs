using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReassignReviewService
{
    private readonly ITaskForReviewRepository _taskForReviewRepository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly INextReviewerStrategyFactory _nextReviewerStrategyFactory;
    
    public ReassignReviewService(
        ITaskForReviewRepository taskForReviewRepository,
        ITeamAccessor teamAccessor,
        IReviewMessageBuilder reviewMessageBuilder,
        INextReviewerStrategyFactory nextReviewerStrategyFactory)
    {
        _taskForReviewRepository =
            taskForReviewRepository ?? throw new ArgumentNullException(nameof(taskForReviewRepository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _reviewMessageBuilder =
            reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _nextReviewerStrategyFactory =
            nextReviewerStrategyFactory ?? throw new ArgumentNullException(nameof(nextReviewerStrategyFactory));
    }
    
    public async Task<IReadOnlyCollection<NotificationMessage>> ReassignReview(
        int messageId,
        Guid taskId,
        BotContext botContext,
        CancellationToken token)
    {
        var task = await _taskForReviewRepository.GetById(taskId, token);
        if (!task.CanAccept())
            return Array.Empty<NotificationMessage>();
        
        var teammates = await _teamAccessor.GetTeammates(task.TeamId, DateTimeOffset.UtcNow, token);
        var nextReviewerType = task.Strategy == NextReviewerType.Target
            ? NextReviewerType.Random
            : task.Strategy;
        var nextReviewerStrategy = await _nextReviewerStrategyFactory.Create(
            task.TeamId,
            task.OwnerId,
            nextReviewerType,
            targetPersonId: null,
            teammates.Select(t => t.Id).ToArray(),
            excludePersonId: task.ReviewerId,
            token);
        
        task.Reassign(DateTimeOffset.UtcNow, nextReviewerStrategy.GetReviewer());
        
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
            botContext,
            token);

        await _taskForReviewRepository.Upsert(task, token);

        return notifications;
    }
}