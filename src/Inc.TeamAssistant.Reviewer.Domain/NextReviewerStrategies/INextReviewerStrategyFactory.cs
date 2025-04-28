namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public interface INextReviewerStrategyFactory
{
    Task<INextReviewerStrategy> Create(
        Guid teamId,
        long ownerId,
        long? reviewerId,
        NextReviewerType nextReviewerType,
        long? targetPersonId,
        IReadOnlyCollection<long> teammates,
        CancellationToken token);
}