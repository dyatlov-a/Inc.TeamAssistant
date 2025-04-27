using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Application.Contracts;
using Inc.TeamAssistant.Reviewer.Domain;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Application.Services;

internal sealed class NextReviewerStrategyFactory : INextReviewerStrategyFactory
{
    private readonly ITaskForReviewReader _reader;

    public NextReviewerStrategyFactory(ITaskForReviewReader reader)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
    }

    public async Task<INextReviewerStrategy> Create(
        Guid teamId,
        long ownerId,
        long? reviewerId,
        NextReviewerType nextReviewerType,
        long? targetPersonId,
        IReadOnlyCollection<long> teammates,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(teammates);
        
        const int historyLimitInDays = 7;
        var fromDate = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(historyLimitInDays));
        var reviewerCandidatePool = await _reader.GetReviewerCandidates(
            teamId,
            fromDate,
            TaskForReviewStateRules.ActiveStates,
            NextReviewerType.Target,
            token);
        var lastReviewerByPerson = reviewerCandidatePool.FirstRoundHistory.SingleOrDefault(r => r.OwnerId == ownerId);
        var lastReviewerByTeam = reviewerCandidatePool.FirstRoundHistory.MaxBy(r => r.Created);

        return nextReviewerType switch
        {
            NextReviewerType.RoundRobin => new RoundRobinReviewerStrategy(new TeammatesPool(
                teammates,
                ownerId,
                reviewerId,
                lastReviewerByPerson?.ReviewerId)),
            NextReviewerType.RoundRobinForTeam => new RoundRobinReviewerStrategy(new TeammatesPool(
                teammates,
                ownerId,
                reviewerId,
                lastReviewerByTeam?.ReviewerId)),
            NextReviewerType.SecondRound => new RoundRobinReviewerStrategy(new TeammatesPool(
                teammates,
                ownerId,
                reviewerId,
                reviewerCandidatePool.SecondRoundHistory)),
            NextReviewerType.Random => new RandomReviewerStrategy(new TeammatesPool(
                teammates,
                ownerId,
                reviewerId,
                lastReviewerByPerson?.ReviewerId),
                reviewerCandidatePool.FirstRoundStats),
            NextReviewerType.Target when targetPersonId.HasValue => new TargetReviewerStrategy(targetPersonId.Value),
            _ => throw new TeamAssistantException($"NextReviewerType {nextReviewerType} was not supported.")
        };
    }
}