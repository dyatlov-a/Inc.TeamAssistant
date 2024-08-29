using Inc.TeamAssistant.Appraiser.Domain.EstimationStrategies;

namespace Inc.TeamAssistant.Appraiser.Domain;

public static class EstimationStrategyFactory
{
    private static readonly IReadOnlyDictionary<StoryType, IEstimationStrategy> Strategies
        = new Dictionary<StoryType, IEstimationStrategy>
        {
            [StoryType.Fibonacci] = new FibonacciEstimationStrategy(),
            [StoryType.TShirt] = new TShirtEstimationStrategy(),
            [StoryType.PowerOfTwo] = new PowerOfTwoEstimationStrategy()
        };

    public static IEnumerable<Estimation> GetAllValues()
    {
        foreach (var value in Estimation.GetValues())
            yield return value;

        foreach (var strategy in Strategies.Values)
        foreach (var value in strategy.GetValues())
            yield return value;
    }

    public static IEstimationStrategy Create(StoryType storyType)
    {
        return Strategies.TryGetValue(storyType, out var strategy)
            ? strategy
            : throw new ArgumentOutOfRangeException(nameof(storyType),
                storyType,
                $"StoryType is not valid for {nameof(EstimationStrategyFactory)}.");
    }
}