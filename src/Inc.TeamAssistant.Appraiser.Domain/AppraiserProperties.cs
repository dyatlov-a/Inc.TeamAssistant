namespace Inc.TeamAssistant.Appraiser.Domain;

public static class AppraiserProperties
{
    public const string StoryTypeKey = "storyType";
    
    public static string GetStoryType(this IReadOnlyDictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        
        return properties.GetValueOrDefault(StoryTypeKey, StoryType.Fibonacci.ToString());
    }
}