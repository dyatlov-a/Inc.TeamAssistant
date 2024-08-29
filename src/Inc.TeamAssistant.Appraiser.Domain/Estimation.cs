namespace Inc.TeamAssistant.Appraiser.Domain;

public sealed record Estimation(
    string Code,
    int Value,
    string DisplayValue,
    string HasValue)
{
    public static readonly Estimation None = new("None", -2, "?", "-");
    public static readonly Estimation More = new("More", -1, "+∞", "-");
    public static readonly Estimation NoIdea = new("NoIdea", 0, "?", "+");

    public static IEnumerable<Estimation> GetValues()
    {
        yield return More;
        yield return NoIdea;
    }

    public static Estimation? GetValue(int value)
    {
        if (None.Value == value)
            return None;
        if (More.Value == value)
            return More;
        if (NoIdea.Value == value)
            return NoIdea;

        return null;
    }
}