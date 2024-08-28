namespace Inc.TeamAssistant.Appraiser.Domain.Estimations;

public sealed record FibonacciEstimation(
    int Value,
    string DisplayValue,
    string HasValue)
    : Estimation(Value, DisplayValue, HasValue)
{
    private static readonly Dictionary<int, (string DisplayValue, string HasValue)> Values = new()
    {
        [None] = ("?", "-"),
        [More] = ("21+", "+"),
        [NoIdea] = ("?", "+"),
        [1] = ("1SP", "+"),
        [2] = ("2SP", "+"),
        [3] = ("3SP", "+"),
        [5] = ("5SP", "+"),
        [8] = ("8SP", "+"),
        [13] = ("13SP", "+"),
        [21] = ("21SP", "+")
    };

    public static Estimation Create(int value)
    {
        if (Values.TryGetValue(value, out var values))
            return new FibonacciEstimation(value, values.DisplayValue, values.HasValue);
        
        throw new ArgumentOutOfRangeException(
            nameof(value),
            value,
            $"Value is not valid for {nameof(FibonacciEstimation)}.");
    }

    public static readonly IReadOnlyCollection<Estimation> Assessments = new[]
    {
        new FibonacciEstimation(1, Values[1].DisplayValue, Values[1].HasValue),
        new FibonacciEstimation(2, Values[2].DisplayValue, Values[2].HasValue),
        new FibonacciEstimation(3, Values[3].DisplayValue, Values[3].HasValue),
        new FibonacciEstimation(5, Values[5].DisplayValue, Values[5].HasValue),
        new FibonacciEstimation(8, Values[8].DisplayValue, Values[8].HasValue),
        new FibonacciEstimation(13, Values[13].DisplayValue, Values[13].HasValue),
        new FibonacciEstimation(21, Values[21].DisplayValue, Values[21].HasValue),
        new FibonacciEstimation(More, Values[More].DisplayValue, Values[More].HasValue),
        new FibonacciEstimation(NoIdea, Values[NoIdea].DisplayValue, Values[NoIdea].HasValue),
    };
}