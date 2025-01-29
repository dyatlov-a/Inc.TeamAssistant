namespace Inc.TeamAssistant.Primitives.Extensions;

public static class StringExtensions
{
    public static bool HasCommand(this string value) => !string.IsNullOrWhiteSpace(value) && value.StartsWith('/');
}