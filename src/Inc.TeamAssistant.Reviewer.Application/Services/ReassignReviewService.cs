using Inc.TeamAssistant.Primitives;
using Inc.TeamAssistant.Primitives.Extensions;
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
    
    public async Task<IReadOnlyCollection<NotificationMessage>> ReassignReview(Guid taskId, CancellationToken token)
    {
        var task = await taskId.Required(_repository.Find, token);
        if (!task.CanAccept())
            return [];
        
        var teammates = await _teamAccessor.GetTeammates(task.TeamId, DateTimeOffset.UtcNow, token);
        var teamContext = await _teamAccessor.GetTeamContext(task.TeamId, token);
        var nextReviewerType = Enum.Parse<NextReviewerType>(teamContext.GetNextReviewerType());
        var nextReviewerStrategy = await _reviewerFactory.Create(
            task.TeamId,
            task.OwnerId,
            nextReviewerType,
            targetPersonId: null,
            teammates.Select(t => t.Id).ToArray(),
            excludePersonId: task.ReviewerId,
            token);
        var nextReviewer = nextReviewerStrategy.GetReviewer();

        await _repository.Upsert(task.Reassign(DateTimeOffset.UtcNow, nextReviewer), token);
        
        return await _reviewMessageBuilder.Build(task, fromOwner: false, token);
    }
}