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

    public static IReadOnlyCollection<int> GetAllValues()
    {
        return Strategies.Values
            .SelectMany(s => s.GetValues().Select(v => v.Value))
            .Distinct()
            .ToArray();
    }

    public static IEstimationStrategy Create(StoryType storyType)
    {
        return Strategies.TryGetValue(storyType, out var strategy)
            ? strategy
            : throw new ArgumentOutOfRangeException(nameof(storyType), storyType, "State out of range.");
    }
}