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
        NextReviewerType nextReviewerType,
        long? targetPersonId,
        IReadOnlyCollection<long> teammates,
        long? excludePersonId,
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
        var excludedPersonIds = excludePersonId.HasValue
            ? new[] { ownerId, excludePersonId.Value }
            : [ownerId];
        
        return nextReviewerType switch
        {
            NextReviewerType.RoundRobin => new RoundRobinReviewerStrategy(
                teammates,
                excludedPersonIds,
                lastReviewerByPerson?.ReviewerId),
            NextReviewerType.RoundRobinForTeam => new RoundRobinReviewerStrategy(
                teammates,
                excludedPersonIds,
                lastReviewerByTeam?.ReviewerId),
            NextReviewerType.SecondRound => new RoundRobinReviewerStrategy(
                teammates,
                excludedPersonIds,
                reviewerCandidatePool.SecondRoundHistory),
            NextReviewerType.Random => new RandomReviewerStrategy(
                teammates,
                reviewerCandidatePool.FirstRoundStats,
                excludedPersonIds,
                lastReviewerByPerson?.ReviewerId),
            NextReviewerType.Target when targetPersonId.HasValue => new TargetReviewerStrategy(targetPersonId.Value),
            _ => throw new TeamAssistantException($"NextReviewerType {nextReviewerType} was not supported.")
        };
    }
}