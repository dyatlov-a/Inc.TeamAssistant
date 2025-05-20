using System.Collections.Immutable;

namespace Inc.TeamAssistant.WebUI.Components;

// TODO: Create builders for all attributes
public static class DynamicAttributes
{
    private static readonly ImmutableDictionary<string, object> Draggable;
    private static readonly ImmutableDictionary<string, object> Contenteditable;
    private static readonly ImmutableDictionary<string, object> OnDragOver;
    
    private const string DisableDefaultHandler = "event.preventDefault();";
    private const string ActiveValue = "true";

    static DynamicAttributes()
    {
        Draggable = Build(("draggable", ActiveValue), ("ondragover", DisableDefaultHandler));
        Contenteditable = Build(("contenteditable", ActiveValue));
        OnDragOver = Build(("ondragover", DisableDefaultHandler));
    }
    
    public static ImmutableDictionary<string, object> BuildDraggable(bool value)
    {
        return value ? Draggable : ImmutableDictionary<string, object>.Empty;
    }
    
    public static ImmutableDictionary<string, object> BuildContenteditable(bool value)
    {
        return value ? Contenteditable : ImmutableDictionary<string, object>.Empty;
    }
    
    public static ImmutableDictionary<string, object> BuildOnDragOver(bool value)
    {
        return value ? OnDragOver : ImmutableDictionary<string, object>.Empty;
    }
    
    private static ImmutableDictionary<string, object> Build(
        params IReadOnlyCollection<(string Key, string Value)> attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        
        var result = attributes
            .ToDictionary(a => a.Key, a => (object)a.Value)
            .ToImmutableDictionary();
        
        return result;
    }
}