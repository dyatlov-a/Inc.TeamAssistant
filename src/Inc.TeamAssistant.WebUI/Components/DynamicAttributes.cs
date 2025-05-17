using System.Collections.Immutable;

namespace Inc.TeamAssistant.WebUI.Components;

public static class DynamicAttributes
{
    private static readonly ImmutableDictionary<string, object> Draggable;
    private static readonly ImmutableDictionary<string, object> Contenteditable;

    static DynamicAttributes()
    {
        Draggable = Build("draggable");
        Contenteditable = Build("contenteditable");
    }

    private static ImmutableDictionary<string, object> Build(string attribute)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(attribute);
        
        return new Dictionary<string, object>
        {
            [attribute] = "true"
        }.ToImmutableDictionary();
    }
    
    public static ImmutableDictionary<string, object> BuildDraggable(bool value)
    {
        return value ? Draggable : ImmutableDictionary<string, object>.Empty;
    }
    
    public static ImmutableDictionary<string, object> BuildContenteditable(bool value)
    {
        return value ? Contenteditable : ImmutableDictionary<string, object>.Empty;
    }
}