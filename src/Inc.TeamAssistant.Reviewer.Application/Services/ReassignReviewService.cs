using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Bots;
using Inc.TeamAssistant.Primitives.Notifications;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class ReassignReviewService
{
    private readonly ITaskForReviewRepository _repository;
    private readonly ITeamAccessor _teamAccessor;
    private readonly IReviewMessageBuilder _reviewMessageBuilder;
    private readonly INextReviewerStrategyFactory _reviewerFactory;
    
    public ReassignReviewService(
        ITaskForReviewRepository repository,
        ITeamAccessor teamAccessor,
        IReviewMessageBuilder reviewMessageBuilder,
        INextReviewerStrategyFactory reviewerFactory)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _teamAccessor = teamAccessor ?? throw new ArgumentNullException(nameof(teamAccessor));
        _reviewMessageBuilder = reviewMessageBuilder ?? throw new ArgumentNullException(nameof(reviewMessageBuilder));
        _reviewerFactory = reviewerFactory ?? throw new ArgumentNullException(nameof(reviewerFactory));
    }
    
    public async Task<IReadOnlyCollection<NotificationMessage>> ReassignReview(
        int messageId,
        Guid taskId,
        BotContext botContext,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(botContext);
        
        var task = await _repository.GetById(taskId, token);
        if (!task.CanAccept())
            return Array.Empty<NotificationMessage>();
        
        var teammates = await _teamAccessor.GetTeammates(task.TeamId, DateTimeOffset.UtcNow, token);
        var teamContext = await _teamAccessor.GetTeamContext(task.TeamId, token);
        var owner = await _teamAccessor.EnsurePerson(task.OwnerId, token);
        var nextReviewerType = Enum.Parse<NextReviewerType>(teamContext.GetNextReviewerType());
        var nextReviewerStrategy = await _reviewerFactory.Create(
            task.TeamId,
            task.OwnerId,
            nextReviewerType,
            targetPersonId: null,
            teammates.Select(t => t.Id).ToArray(),
            excludePersonId: task.ReviewerId,
            token);
        var newReviewerId = task.Reassign(DateTimeOffset.UtcNow, nextReviewerStrategy.GetReviewer());
        var newReviewer = await _teamAccessor.EnsurePerson(newReviewerId, token);

        await _repository.Upsert(task, token);
        
        return await _reviewMessageBuilder.Build(
            messageId,
            task,
            newReviewer,
            owner,
            botContext,
            token);
    }
}