namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed record Estimation(
    int Value,
    string DisplayValue,
    string HasValue)
{
    private static readonly IReadOnlyDictionary<int, Estimation> Estimations = new Dictionary<int, Estimation>
    {
        { -1, new Estimation(-1, "+∞", "+") },
        { 0, new(0, "?", "+") }
    };
    
    public static readonly Estimation None = new(-2, "?", "-");
    public static readonly Estimation NoIdea = new(0, "?", "+");

    public static IEnumerable<Estimation> GetValues() => Estimations.Values;

    public static Estimation? GetValue(int value) => Estimations.GetValueOrDefault(value);
}