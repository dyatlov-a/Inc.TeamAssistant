namespace Inc.TeamAssistant.Appraiser.Domain.Estimations;

public sealed record TShirtEstimation(
    int Value,
    string DisplayValue,
    string HasValue)
    : Estimation(Value, DisplayValue, HasValue)
{
    private static readonly Dictionary<int, (string DisplayValue, string HasValue)> Values = new()
    {
        [None] = ("?", "-"),
        [More] = ("XXL+", "+"),
        [NoIdea] = ("?", "+"),
        [100] = ("XS", "+"),
        [200] = ("S", "+"),
        [300] = ("M", "+"),
        [400] = ("L", "+"),
        [500] = ("XL", "+"),
        [600] = ("XXL", "+")
    };
    
    public static Estimation Create(int value)
    {
        if (Values.TryGetValue(value, out var values))
            return new FibonacciEstimation(value, values.DisplayValue, values.HasValue);
        
        throw new ArgumentOutOfRangeException(
            nameof(value),
            value,
            $"Value is not valid for {nameof(TShirtEstimation)}.");
    }
    
    public static readonly IReadOnlyCollection<Estimation> Assessments = new[]
    {
        new FibonacciEstimation(100, Values[100].DisplayValue, Values[100].HasValue),
        new FibonacciEstimation(200, Values[200].DisplayValue, Values[200].HasValue),
        new FibonacciEstimation(300, Values[300].DisplayValue, Values[300].HasValue),
        new FibonacciEstimation(400, Values[400].DisplayValue, Values[400].HasValue),
        new FibonacciEstimation(500, Values[500].DisplayValue, Values[500].HasValue),
        new FibonacciEstimation(600, Values[600].DisplayValue, Values[600].HasValue),
        new FibonacciEstimation(More, Values[More].DisplayValue, Values[More].HasValue),
        new FibonacciEstimation(NoIdea, Values[NoIdea].DisplayValue, Values[NoIdea].HasValue),
    };
}