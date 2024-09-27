using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.WebUI.Routing;

public sealed record NavRoute(
    LanguageId? LanguageId,
    string RouteSegment)
{
    public override string ToString()
    {
        var languageSegment = LanguageId is null
            ? string.Empty
            : $"/{LanguageId.Value}";
        
        return RouteSegment.StartsWith('/')
            ? $"{languageSegment}{RouteSegment}"
            : $"{languageSegment}/{RouteSegment}";
    }
    
    public static implicit operator string(NavRoute counter) => counter.ToString();
}