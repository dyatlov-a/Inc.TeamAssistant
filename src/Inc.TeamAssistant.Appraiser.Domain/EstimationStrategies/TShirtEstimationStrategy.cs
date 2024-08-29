namespace Inc.TeamAssistant.Appraiser.Domain.EstimationStrategies;

internal sealed class TShirtEstimationStrategy : IEstimationStrategy
{
    private static readonly IReadOnlyDictionary<int, Estimation> Estimations = new Dictionary<int, Estimation>
    {
        [100] = new(100, "XS", "+"),
        [200] = new(200, "S", "+"),
        [300] = new(300, "M", "+"),
        [400] = new(400, "L", "+"),
        [500] = new(500, "XL", "+"),
        [600] = new(600, "XXL", "+")
    };

    public IEnumerable<Estimation> GetValues() => Estimations.Values;
    
    public Estimation GetValue(int value)
    {
        var defaultEstimation = Estimation.GetValue(value);
        if (defaultEstimation is not null)
            return defaultEstimation;
        
        if (Estimations.TryGetValue(value, out var estimation))
            return estimation;
        
        throw new ArgumentOutOfRangeException(
            nameof(value),
            value,
            $"Value is not valid for {nameof(TShirtEstimationStrategy)}.");
    }

    public Estimation CalculateMean(Story story) => CalculateMedian(story);

    public Estimation CalculateMedian(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);
		
        var values = story.StoryForEstimates
            .Where(i => i.Value > Estimation.NoIdea.Value)
            .OrderBy(i => i.Value)
            .Select(i => GetValue(i.Value))
            .ToArray();

        if (!values.Any())
            return Estimation.None;

        var middle = values.Length / 2;
        return values[middle];
    }
}