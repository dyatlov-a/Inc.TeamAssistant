namespace Inc.TeamAssistant.Appraiser.Domain.EstimationStrategies;

internal sealed class PowerOfTwoEstimationStrategy : IEstimationStrategy
{
    private static readonly IReadOnlyDictionary<int, Estimation> Estimations = new Dictionary<int, Estimation>
    {
        { 1, new("1SP", 1, "1 SP", "+") },
        { 2, new("2SP", 2, "2 SP", "+") },
        { 4, new("4SP", 4, "4 SP", "+") },
        { 8, new("8SP", 8, "8 SP", "+") },
        { 16, new("16SP", 16, "16 SP", "+") },
        { 32, new("32SP", 32, "32 SP", "+") },
        { 64, new("64SP", 64, "64 SP", "+") }
    };
    
    public IEnumerable<Estimation> GetValues()
    {
        foreach (var estimation in Estimations.Values)
            yield return estimation;
        
        foreach (var estimation in Estimation.GetValues())
            yield return estimation;
    }

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
    
    public int GetWeight(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);

        return story.TotalValue ?? 0;
    }

    public Estimation CalculateMean(Story story)
    {
        ArgumentNullException.ThrowIfNull(story);
        
        if (!story.EstimateEnded)
            return Estimation.None;
		
        var values = story.StoryForEstimates
            .Where(i => i.Value > Estimation.NoIdea.Value)
            .Select(i => i.Value)
            .ToArray();

        var mean = values.Any()
            ? (decimal?)values.Sum() / values.Length
            : null;

        return mean.HasValue
            ? new Estimation(string.Empty, (int)mean.Value, ValueToString(mean.Value), "+")
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

        return new Estimation(string.Empty, (int)median, ValueToString(median), "+");
    }
    
    private string ValueToString(decimal value) => value.ToString(".## SP");
}