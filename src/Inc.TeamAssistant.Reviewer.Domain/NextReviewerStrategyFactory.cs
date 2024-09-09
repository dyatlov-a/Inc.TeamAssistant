using Inc.TeamAssistant.Primitives.Exceptions;
using Inc.TeamAssistant.Reviewer.Domain.NextReviewerStrategies;

namespace Inc.TeamAssistant.Reviewer.Domain;

internal static class NextReviewerStrategyFactory
{
    public static INextReviewerStrategy Create(
        NextReviewerType nextReviewerType,
        IReadOnlyCollection<long> teammates,
        IReadOnlyDictionary<long, int> history)
    {
        ArgumentNullException.ThrowIfNull(teammates);
        ArgumentNullException.ThrowIfNull(history);
        
        return nextReviewerType switch
        {
            NextReviewerType.RoundRobin => new RoundRobinReviewerStrategy(teammates),
            NextReviewerType.Random => new RandomReviewerStrategy(teammates, history),
            _ => throw new TeamAssistantException($"Strategy {nextReviewerType} was not supported.")
        };
    }
}