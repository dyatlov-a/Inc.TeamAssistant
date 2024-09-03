namespace Inc.TeamAssistant.Appraiser.Domain.EstimationStrategies;

internal sealed class FibonacciEstimationStrategy : IEstimationStrategy
{
    private static readonly IReadOnlyDictionary<int, Estimation> Estimations = new Dictionary<int, Estimation>
    {
        { 1, new("1SP", 1, "1SP", "+") },
        { 2, new("2SP", 2, "2SP", "+") },
        { 3, new("3SP", 3, "3SP", "+") },
        { 5, new("5SP", 5, "5SP", "+") },
        { 8, new("8SP", 8, "8SP", "+") },
        { 13, new("13SP", 13, "13SP", "+") },
        { 21, new("21SP", 21, "21SP", "+") }
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