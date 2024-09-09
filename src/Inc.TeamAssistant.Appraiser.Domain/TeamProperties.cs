using Inc.TeamAssistant.Primitives.Commands;

namespace Inc.TeamAssistant.Appraiser.Domain;

public static class TeamProperties
{
    public const string StoryTypeKey = "storyType";
    
    public static string GetStoryType(this CurrentTeamContext teamContext)
    {
        ArgumentNullException.ThrowIfNull(teamContext);
        
        return teamContext.Properties.GetValueOrDefault(StoryTypeKey, StoryType.Fibonacci.ToString());
    }
}