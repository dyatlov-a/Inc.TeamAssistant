namespace Inc.TeamAssistant.Appraiser.Domain.EstimationStrategies;

internal sealed class PowerOfTwoEstimationStrategy : IEstimationStrategy
{
    private static readonly IReadOnlyDictionary<int, Estimation> Estimations = new Dictionary<int, Estimation>
    {
        { 1, new(1, "1SP", "+") },
        { 2, new(2, "2SP", "+") },
        { 4, new(4, "4SP", "+") },
        { 8, new(8, "8SP", "+") },
        { 16, new(16, "16SP", "+") },
        { 32, new(32, "32SP", "+") },
        { 64, new(64, "64SP", "+") }
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
            $"Value is not valid for {nameof(FibonacciEstimationStrategy)}.");
    }

    public Estimation CalculateMean(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);
		
        var values = story.StoryForEstimates
            .Where(i => i.Value > Estimation.NoIdea.Value)
            .Select(i => i.Value)
            .ToArray();

        var mean = values.Any()
            ? (decimal?)values.Sum() / values.Length
            : null;

        return mean.HasValue
            ? new Estimation((int)mean.Value, ValueToString(mean.Value), "+")
            : Estimation.None;
    }

    public Estimation CalculateMedian(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);
        
        if (!story.EstimateEnded)
            return Estimation.None;

        var values = story.StoryForEstimates
            .Where(i => i.Value > Estimation.NoIdea.Value)
            .OrderBy(i => i.Value)
            .Select(i => i.Value)
            .ToArray();

        if (!values.Any())
            return Estimation.None;

        var middle = values.Length / 2;

        var median = values.Length % 2 == 0
            ? (values[middle] + values[middle - 1]) / 2m
            : values[middle];

        return new Estimation((int)median, ValueToString(median), "+");
    }
    
    private string ValueToString(decimal value) => value.ToString(".## SP");
}