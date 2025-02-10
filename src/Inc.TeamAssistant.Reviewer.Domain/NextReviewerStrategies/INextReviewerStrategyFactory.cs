namespace Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

public interface INextReviewerStrategyFactory
{
    Task<INextReviewerStrategy> Create(
        Guid teamId,
        long ownerId,
        NextReviewerType nextReviewerType,
        long? targetPersonId,
        IReadOnlyCollection<long> teammates,
        long? excludePersonId,
        CancellationToken token);
}