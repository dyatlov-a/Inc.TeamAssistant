using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Domain;

public static class TeamProperties
{
    public const string StoryTypeKey = "storyType";
    
    private static readonly string StoryTypeDefault = StoryType.Fibonacci.ToString();
    
    public static string GetStoryType(this CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(teamContext);
        
        return teamContext.Properties.GetValueOrDefault(StoryTypeKey, StoryTypeDefault);
    }
}