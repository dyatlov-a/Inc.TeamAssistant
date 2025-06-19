namespace Inc.TeamAssistant.WebUI.Extensions;

internal static class StringExtensions
{
    private const int DefaultLength = 50;
    
    public static string ToShort(this string? value, int length = DefaultLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        
        return value.Length <= length
            ? value
            : value[..length] + "...";
    }
}