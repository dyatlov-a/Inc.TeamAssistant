namespace Inc.TeamAssistant.Appraiser.Domain;

public abstract record Estimation(
    int Value,
    string DisplayValue,
    string HasValue)
{
    public static string UnknownValue = "?";
    
    public static int None = -2;
    public static int More = -1;
    public static int NoIdea = 0;
}